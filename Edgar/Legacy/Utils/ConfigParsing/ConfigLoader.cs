using MapGeneration.Core.Doors.Interfaces;
using MapGeneration.Core.MapDescriptions.Interfaces;

namespace MapGeneration.Utils.ConfigParsing
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Core;
	using Core.Doors.DoorModes;
	using Core.MapDescriptions;
	using Deserializers;
	using GeneralAlgorithms.DataStructures.Common;
	using GeneralAlgorithms.DataStructures.Polygons;
	using Models;
	using YamlDotNet.Serialization;
	using YamlDotNet.Serialization.NamingConventions;
	using YamlDotNet.Serialization.NodeDeserializers;

	/// <summary>
	/// Class that loads MapDescription from a file.
	/// </summary>
	public class ConfigLoader
	{
		private readonly IDeserializer deserializer;

		/// <summary>
		/// Path to the folder with room configs.
		/// </summary>
		private const string RoomsPath = "Resources/Rooms";

		/// <summary>
		/// Path to the folder with map configs.
		/// </summary>
		private const string MapsPath = "Resources/Maps";

		private const string CustomSetName = "custom";

        private Dictionary<RoomTemplateIdentifier, RoomTemplate> roomTemplates;

		public ConfigLoader()
		{
			deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.WithNodeDeserializer(new IntVector2Deserializer(), s => s.OnTop())
				.WithNodeDeserializer(new OrthogonalLineDeserializer(), s => s.OnTop())
				.WithNodeDeserializer(new StringTupleDeserializer(), s => s.OnTop())
				.WithTagMapping("!OverlapMode", typeof(SimpleDoorModeModel))
				.WithTagMapping("!SimpleMode", typeof(SimpleDoorModeModel))
				.WithTagMapping("!SpecificPositionsMode", typeof(ManualDoorModeModel))
				.WithTagMapping("!ManualMode", typeof(ManualDoorModeModel))
				.WithObjectFactory(new DefaultObjectFactory())
				.Build();
		}

		/// <summary>
		/// Loads RoomDescriptionsSetModel from a given TextReader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public RoomDescriptionsSetModel LoadRoomDescriptionsSetModel(TextReader reader)
		{
			return deserializer.Deserialize<RoomDescriptionsSetModel>(reader);
		}

		/// <summary>
		/// Loads MapDescriptionModel from a given TextReader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public MapDescriptionModel LoadMapDescriptionModel(TextReader reader)
		{
			return deserializer.Deserialize<MapDescriptionModel>(reader);
		}

		/// <summary>
		/// Tries to load MapDescription from a file from the maps folder.
		/// </summary>
		/// <param name="name">Name of the file in the maps folder</param>
		/// <returns></returns>
		public MapDescription<int> LoadMapDescriptionFromResources(string name)
		{
			using (var sr = new StreamReader($"{MapsPath}/{name}"))
			{
				return LoadMapDescription(sr);
			}
		}

		/// <summary>
		/// Loads MapDescription from a given TextReader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public MapDescription<int> LoadMapDescription(TextReader reader)
		{
			roomTemplates = new Dictionary<RoomTemplateIdentifier, RoomTemplate>();
			var mapDescriptionModel = LoadMapDescriptionModel(reader);
			var mapDescription = new MapDescription<int>();
			var roomDescriptionsSets = LoadRoomDescriptionsSetsFromResources();

			if (mapDescriptionModel.CustomRoomDescriptionsSet != null)
			{
				roomDescriptionsSets.Add(CustomSetName, mapDescriptionModel.CustomRoomDescriptionsSet);
			}
			
			LoadRooms(mapDescription, mapDescriptionModel, roomDescriptionsSets);
			LoadPassages(mapDescription, mapDescriptionModel, roomDescriptionsSets);

            return mapDescription;
		}

		/// <summary>
		/// Loads MapDescription from a given file.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public MapDescription<int> LoadMapDescription(string filename)
		{
			using (var sr = new StreamReader(filename))
			{
				return LoadMapDescription(sr);
			}
		}

		/// <summary>
		/// Gets a list of yaml files inside the maps resources folder.
		/// </summary>
		/// <returns></returns>
		public List<string> GetSavedMapDescriptionsNames()
		{
			return Directory.GetFiles(MapsPath, "*.yml").Select(Path.GetFileName).ToList();
		}

		/// <summary>
		/// Loads all RoomDescriptionsSetModels from the rooms resources folder.
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, RoomDescriptionsSetModel> LoadRoomDescriptionsSetsFromResources()
		{
			var filenames = Directory.GetFiles(RoomsPath, "*.yml");
			var models = new Dictionary<string, RoomDescriptionsSetModel>();

			foreach (var filename in filenames)
			{
				using (var sr = new StreamReader(filename))
				{
					var model = LoadRoomDescriptionsSetModel(sr);

					if (string.IsNullOrEmpty(model.Name))
					{
						throw new InvalidOperationException("Name must not be empty");
					}

					if (model.Name == CustomSetName)
					{
						throw new InvalidOperationException($"'{CustomSetName}' is a reserved set name");
					}

					models.Add(model.Name, model);

                    foreach (var pair in model.RoomDescriptions)
                    {
                        var name = pair.Key;
                        var roomDescription = pair.Value;
                        roomDescription.Name = name;
                    }
				}
			}

            return models;
		}

		private void LoadPassages(MapDescription<int> mapDescription, MapDescriptionModel mapDescriptionModel, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets)
		{
			if (mapDescriptionModel.Passages == null)
				return;

            var corridors = GetCorridors(mapDescriptionModel, roomDescriptionsSets);
            var corridorsCounter = mapDescription.GetGraph().Vertices.Max() + 1;

			foreach (var passage in mapDescriptionModel.Passages)
			{
                if (corridors != null)
                {
                    var corridorRoom = corridorsCounter++;
					mapDescription.AddRoom(corridorRoom, corridors);

					mapDescription.AddConnection(passage.X, corridorRoom);
					mapDescription.AddConnection(passage.Y, corridorRoom);
                }
                else
                {
                    mapDescription.AddConnection(passage.X, passage.Y);
                }
            }
		}

		private CorridorRoomDescription GetCorridors(MapDescriptionModel mapDescriptionModel, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets)
		{
			if (mapDescriptionModel.Corridors == null)
				return null;

			var corridors = mapDescriptionModel.Corridors;
			var enable = corridors.Enable ?? true;

            if (!enable)
            {
                return null;
            }
			
			if (enable && (corridors.CorridorShapes == null || corridors.CorridorShapes.Count == 0))
				throw new InvalidOperationException("There must be at least one shape for corridors if they are enabled.");

			var roomTemplates = GetRoomTemplates(corridors.CorridorShapes, roomDescriptionsSets, mapDescriptionModel.CustomRoomDescriptionsSet);
            var roomDescription = new CorridorRoomDescription(roomTemplates);

            return roomDescription;
        }

		private void LoadRooms(MapDescription<int> mapDescription, MapDescriptionModel mapDescriptionModel, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets)
		{
			if (mapDescriptionModel.RoomsRange == null)
				throw new InvalidOperationException("Rooms range must be defined");

			if (mapDescriptionModel.RoomsRange.To - mapDescriptionModel.RoomsRange.From <= 0)
				throw new InvalidOperationException("There must be at least one roon in the room range. 'To' must be greater than 'From'.");

			var notUsedRooms = new List<int>();
			for (var i = mapDescriptionModel.RoomsRange.From; i <= mapDescriptionModel.RoomsRange.To; i++)
			{
                notUsedRooms.Add(i);
			}

			if (mapDescriptionModel.Rooms != null)
			{
				foreach (var pair in mapDescriptionModel.Rooms)
				{
					var roomModel = pair.Value;
                    var rooms = pair.Key;

					if (roomModel.RoomShapes != null)
                    {
                        var roomTemplates = GetRoomTemplates(roomModel.RoomShapes, roomDescriptionsSets, mapDescriptionModel.CustomRoomDescriptionsSet);
                        var roomDescription = new BasicRoomDescription(roomTemplates);

                        foreach (var room in rooms)
                        {
                            mapDescription.AddRoom(room, roomDescription);
                            notUsedRooms.Remove(room);
                        }
					}
				}
			}

            if (notUsedRooms.Count != 0)
            {
                if (mapDescriptionModel.DefaultRoomShapes != null)
                {
                    var roomTemplates = GetRoomTemplates(mapDescriptionModel.DefaultRoomShapes, roomDescriptionsSets, mapDescriptionModel.CustomRoomDescriptionsSet);
                    var roomDescription = new BasicRoomDescription(roomTemplates);

                    foreach (var room in notUsedRooms)
                    {
                        mapDescription.AddRoom(room, roomDescription);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Default room shapes must be provided if there are rooms that do not have their own room shapes assigned. Rooms with missing room shapes: {string.Join(", ", notUsedRooms)}");
                }
            }
        }

        private List<Transformation> GetTransformations(bool? rotate)
		{
			return (rotate.HasValue && rotate.Value == false)
                ? new List<Transformation>() {Transformation.Identity}
                : new List<Transformation>()
            {
				Transformation.Identity,
				Transformation.Rotate90,
				Transformation.Rotate180,
				Transformation.Rotate270
            };
		}

		private List<RoomTemplate> GetRoomTemplates(List<RoomShapesModel> roomShapesModels, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets, RoomDescriptionsSetModel customRoomDescriptionsSet)
        {
            var roomTemplates = new List<RoomTemplate>();

            foreach (var roomShapesModel in roomShapesModels)
            {
                roomTemplates.AddRange(GetRoomTemplates(roomShapesModel, roomDescriptionsSets, customRoomDescriptionsSet));
            }

            return roomTemplates;
        }

        private List<RoomTemplate> GetRoomTemplates(RoomShapesModel roomShapesModel, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets, RoomDescriptionsSetModel customRoomDescriptionsSet)
		{
			var setModel = GetSetModel(roomShapesModel.SetName, roomDescriptionsSets, customRoomDescriptionsSet);
			var setName = string.IsNullOrEmpty(roomShapesModel.SetName) ? "custom" : roomShapesModel.SetName;
            var scale = roomShapesModel.Scale;
            var transformations = GetTransformations(roomShapesModel.Rotate);

			if (!string.IsNullOrEmpty(roomShapesModel.RoomDescriptionName) && !setModel.RoomDescriptions.ContainsKey(roomShapesModel.RoomDescriptionName))
				throw new ParsingException($"Room description with name \"{roomShapesModel.RoomDescriptionName}\" was not found in the set \"{setName}\"");

			var roomModels = GetFilledRoomModels(setModel, roomShapesModel.RoomDescriptionName, setName);
			var roomTemplates = roomModels.Select(x => ConvertRoomModelToRoomTemplate(x, scale, transformations, setName)).ToList();

			return roomTemplates;
		}

		private RoomTemplate ConvertRoomModelToRoomTemplate(RoomDescriptionModel model, Vector2Int? scale, List<Transformation> transformations, string setName)
		{
			var identifier = new RoomTemplateIdentifier()
            {
				RoomDescriptionName = model.Name,
				SetName = setName,
				Scale = scale ?? new Vector2Int(1, 1),
            };

            if (roomTemplates.ContainsKey(identifier))
            {
                return roomTemplates[identifier];
            }

			var roomTemplate = new RoomTemplate(new PolygonGrid2D(model.Shape).Scale(scale ?? new Vector2Int(1, 1)), GetDoorMode(model.DoorMode), transformations, model.RepeatMode ?? RepeatMode.AllowRepeat);
            roomTemplates.Add(identifier, roomTemplate);

            return roomTemplate;
        }

        private IDoorMode GetDoorMode(IDoorModeModel model)
        {
            if (model is SimpleDoorModeModel simpleDoorMode)
            {
                return new SimpleDoorMode(simpleDoorMode.DoorLength, simpleDoorMode.CornerDistance);
            }
            else if (model is ManualDoorModeModel manualDoorMode)
            {
                return new ManualDoorMode(manualDoorMode.DoorPositions);
            }

			throw new InvalidOperationException("Invalid door mode");
        }

		private List<RoomDescriptionModel> GetFilledRoomModels(RoomDescriptionsSetModel setModel, string roomDescriptionName, string setName)
		{
			var roomModels = new List<RoomDescriptionModel>();
			var defaultModel = setModel.Default;

			foreach (var pair in setModel.RoomDescriptions)
			{
				var name = pair.Key;
				var roomModel = pair.Value;
                roomModel.Name = name;

				if (!string.IsNullOrEmpty(roomDescriptionName) && name != roomDescriptionName)
					continue;

				if (roomModel.Shape == null)
				{
					if (defaultModel.Shape == null)
						throw new ParsingException($"Neither shape nor default shape are set. Room {name}, set {setName}");

					roomModel.Shape = defaultModel.Shape;
				}

				if (roomModel.DoorMode == null)
				{
					if (defaultModel.DoorMode == null)
						throw new ParsingException($"Neither door mode nor default door mode are set. Room {name}, set {setName}");

					roomModel.DoorMode = defaultModel.DoorMode;
				}

				roomModels.Add(roomModel);
			}

			return roomModels;
		}

		private RoomDescriptionsSetModel GetSetModel(string name, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets, RoomDescriptionsSetModel customRoomDescriptionsSet)
		{
			if (string.IsNullOrEmpty(name))
			{
				return customRoomDescriptionsSet;
			}

			if (!roomDescriptionsSets.TryGetValue(name, out var set))
				throw new ParsingException($"Room descriptions set with name \"{name}\" was not found");

			return set;
		}
	}
}