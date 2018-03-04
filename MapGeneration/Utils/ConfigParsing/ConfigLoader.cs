namespace MapGeneration.Utils.ConfigParsing
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Core;
	using Core.Doors.DoorModes;
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
		private readonly Deserializer deserializer;

		/// <summary>
		/// Path to the folder with room configs.
		/// </summary>
		private const string RoomsPath = "Resources/Rooms";

		/// <summary>
		/// Path to the folder with map configs.
		/// </summary>
		private const string MapsPath = "Resources/Maps";

		public ConfigLoader()
		{
			deserializer = new DeserializerBuilder()
				.WithNamingConvention(new CamelCaseNamingConvention())
				.WithNodeDeserializer(new IntVector2Deserializer(), s => s.OnTop())
				.WithNodeDeserializer(new OrthogonalLineDeserializer(), s => s.OnTop())
				.WithNodeDeserializer(new StringTupleDeserializer(), s => s.OnTop())
				.WithTagMapping("!OverlapMode", typeof(OverlapMode))
				.WithTagMapping("!SpecificPositionsMode", typeof(SpecificPositionsMode))
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
			var mapDescriptionModel = LoadMapDescriptionModel(reader);
			var mapDescription = new MapDescription<int>();
			var roomDescriptionsSets = LoadRoomDescriptionsSetsFromResources();

			LoadRooms(mapDescription, mapDescriptionModel, roomDescriptionsSets);
			LoadPassagess(mapDescription, mapDescriptionModel);
			LoadCorridors(mapDescription, mapDescriptionModel, roomDescriptionsSets);

			return mapDescription;
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

					models.Add(model.Name, model);
				}
			}

			return models;
		}

		private void LoadPassagess(MapDescription<int> mapDescription, MapDescriptionModel mapDescriptionModel)
		{
			if (mapDescriptionModel.Passages == null)
				return;

			foreach (var passage in mapDescriptionModel.Passages)
			{
				mapDescription.AddPassage(passage.X, passage.Y);
			}
		}

		private void LoadCorridors(MapDescription<int> mapDescription, MapDescriptionModel mapDescriptionModel, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets)
		{
			if (mapDescriptionModel.Corridors == null)
				return;

			var corridors = mapDescriptionModel.Corridors;
			var enable = corridors.Enable ?? true;

			if (enable && (corridors.Offsets == null || corridors.Offsets.Count == 0))
				throw new InvalidOperationException("There must be at least one offset if corridors are enabled");

			mapDescription.SetWithCorridors(enable, corridors.Offsets);

			if (enable && (corridors.CorridorShapes == null || corridors.CorridorShapes.Count == 0))
				throw new InvalidOperationException("There must be at least one shape for corridors if they are enabled.");

			foreach (var rooms in corridors.CorridorShapes)
			{
				var roomShapes = GetRoomDescriptions(rooms, roomDescriptionsSets, mapDescriptionModel.CustomRoomDescriptionsSet, rooms.Scale);
				mapDescription.AddCorridorShapes(roomShapes, rooms.Rotate ?? true, rooms.Probability ?? 1, rooms.NormalizeProbabilities ?? true);
			}
		}

		private void LoadRooms(MapDescription<int> mapDescription, MapDescriptionModel mapDescriptionModel, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets)
		{
			if (mapDescriptionModel.RoomsRange == null)
				throw new InvalidOperationException("Rooms range must be defined");

			if (mapDescriptionModel.RoomsRange.To - mapDescriptionModel.RoomsRange.From <= 0)
				throw new InvalidOperationException("There must be at least one roon in the room range. 'To' must be greater than 'From'.");

			for (var i = mapDescriptionModel.RoomsRange.From; i <= mapDescriptionModel.RoomsRange.To; i++)
			{
				mapDescription.AddRoom(i);
			}

			if (mapDescriptionModel.Rooms != null)
			{
				foreach (var pair in mapDescriptionModel.Rooms)
				{
					var room = pair.Value;

					if (room.RoomShapes != null)
					{
						foreach (var rooms in room.RoomShapes)
						{
							var roomShapes = GetRoomDescriptions(rooms, roomDescriptionsSets, mapDescriptionModel.CustomRoomDescriptionsSet, rooms.Scale);

							foreach (var name in pair.Key)
							{
								mapDescription.AddRoomShapes(name, roomShapes, rooms.Rotate ?? true, rooms.Probability ?? 1, rooms.NormalizeProbabilities ?? true);
							}
						}
					}
				}
			}

			if (mapDescriptionModel.DefaultRoomShapes != null)
			{
				foreach (var rooms in mapDescriptionModel.DefaultRoomShapes)
				{
					var roomShapes = GetRoomDescriptions(rooms, roomDescriptionsSets, mapDescriptionModel.CustomRoomDescriptionsSet, rooms.Scale);
					mapDescription.AddRoomShapes(roomShapes, rooms.Rotate ?? true, rooms.Probability ?? 1, rooms.NormalizeProbabilities ?? true);
				}
			}
		}

		private List<RoomDescription> GetRoomDescriptions(RoomShapesModel roomShapesModel, Dictionary<string, RoomDescriptionsSetModel> roomDescriptionsSets, RoomDescriptionsSetModel customRoomDescriptionsSet, IntVector2? scale)
		{
			var setModel = GetSetModel(roomShapesModel.SetName, roomDescriptionsSets, customRoomDescriptionsSet);
			var setName = string.IsNullOrEmpty(roomShapesModel.SetName) ? "custom" : roomShapesModel.SetName;

			if (!string.IsNullOrEmpty(roomShapesModel.RoomDescriptionName) && !setModel.RoomDescriptions.ContainsKey(roomShapesModel.RoomDescriptionName))
				throw new ParsingException($"Room description with name \"{roomShapesModel.RoomDescriptionName}\" was not found in the set \"{setName}\"");

			var roomModels = GetFilledRoomModels(setModel, roomShapesModel.RoomDescriptionName, setName);
			var roomDescriptions = roomModels.Select(x => ConvertRoomModelToDescription(x, scale)).ToList();

			return roomDescriptions;
		}

		private RoomDescription ConvertRoomModelToDescription(RoomDescriptionModel model, IntVector2? scale)
		{
			return new RoomDescription(new GridPolygon(model.Shape).Scale(scale ?? new IntVector2(1, 1)), model.DoorMode);
		}

		private List<RoomDescriptionModel> GetFilledRoomModels(RoomDescriptionsSetModel setModel, string roomDescriptionName, string setName)
		{
			var roomModels = new List<RoomDescriptionModel>();
			var defaultModel = setModel.Default;

			foreach (var pair in setModel.RoomDescriptions)
			{
				var name = pair.Key;
				var roomModel = pair.Value;

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