namespace VConSharp.Examples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("VConSharp Examples");
            Console.WriteLine("=================");
            Console.WriteLine();

            // Run simple example
            SimpleExample.Run();

            // Run audio/video example
            Console.WriteLine();
            AudioVideoExample.Run();
        }
    }
}
