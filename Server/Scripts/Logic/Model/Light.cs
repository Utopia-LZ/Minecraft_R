public class Light : Entity
{
    public static int radius;

    public static void InitConfig(Config config)
    {
        radius = config.LightRadius;
    }
}