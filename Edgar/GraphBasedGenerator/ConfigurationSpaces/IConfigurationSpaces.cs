namespace Edgar.GraphBasedGenerator.ConfigurationSpaces
{
    public interface IConfigurationSpaces<in TConfiguration>
    {
        bool HaveValidPosition(TConfiguration configuration1, TConfiguration configuration2);
    }
}