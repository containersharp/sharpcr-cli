namespace SharpCR.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            new Docker().Fire(args);
        }
    }
}
