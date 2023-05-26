using NetworkCalculator;

public class Program
{
    private static void Main(string[] args)
    {
        while(true)
        {
            try
            {
                Console.Write("IP-Adresse oder NetzId mit CIDR Notation angeben (z.b. 192.168.0.0/24): ");
                string userInput = Console.ReadLine();
                Network network = new Network(userInput);
                network.ConsoleWrite();
            }
            catch (Exception)
            {
                Console.WriteLine("Ungültige Eingabe.");
            }

        }

    }
}