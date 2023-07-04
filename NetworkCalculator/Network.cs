using System.Net;
using System.Text;

namespace NetworkCalculator
{
    public class Subnet
    {
        private string Name { get; set; }
        private string NetworkAddress { get; set; }
        private string BinarySubnetAddress { get; set; }
        private string NetworkId { get; set; }

        public Subnet(string networkAddress, int cidr)
        {
            NetworkAddress = networkAddress;
            BinarySubnetAddress = Network.CalculateBinarySubnet(cidr);
        }

    }


    public class Network
    {
        private string IpAddress { get; set; }
        private int Cidr { get; set; }
        private string BinaryIpAddress { get; set; }
        private string BinarySubnetAddress { get; set; }
        private string BinaryNetworkId { get; set; }
        private string BinaryBroadcastAddress { get; set; }

        private string SubnetAddress { get; set; }
        private string NetworkId { get; set; }
        private string BroadcastAddress { get; set; }

        private int AvailableHosts { get; set; }
        private string availableFrom;
        private string availableTo;

        /// <summary>
        /// Initializes this class.
        /// Calculates all addresses from the given [IP-Address/CIDR] and saves them into class variables.
        /// Use ConsoleWrite() Method to get calculated output.
        /// </summary>
        /// <param name="ipCidr"></param>
        /// <exception cref="Exception"></exception>
        public Network(string ipCidr)
        {
            ipCidr = ipCidr.Replace(" ", "");

            IpAddress = ipCidr.Split("/")[0];
            Cidr = int.Parse(ipCidr.Split("/")[1]);

            if (IpAddress.Contains("+") || ipCidr.Contains("-"))
                throw new Exception();

            if (Cidr < 0 || Cidr > 31)
                throw new Exception();

            BinaryIpAddress = ConvertToBinary(IpAddress);
            BinarySubnetAddress = CalculateBinarySubnet(Cidr);
            BinaryNetworkId = CalculateBinaryNetworkID(BinaryIpAddress);
            BinaryBroadcastAddress = CalculateBinaryBroadcastAddress(BinaryIpAddress);

            SubnetAddress = ConvertToIPAddress(BinarySubnetAddress);
            NetworkId = ConvertToIPAddress(BinaryNetworkId);
            BroadcastAddress = ConvertToIPAddress(BinaryBroadcastAddress);

            AvailableHosts = CalculateAvailableHosts();
            CalculateAvailableHostAddressRange(out availableFrom, out availableTo);
        }

        /// <summary>
        /// Calculates all available hosts through cidr formula.
        /// </summary>
        /// <returns>integer of available hosts</returns>
        private int CalculateAvailableHosts()
        {
            return (int)(Math.Pow(2, 32 - Cidr) - 2);
        }

        /// <summary>
        /// Calculates the binary network id through cidr and a given binary IP-Address.
        /// </summary>
        /// <param name="binaryIpAddress"></param>
        /// <returns></returns>
        private string CalculateBinaryNetworkID(string binaryIpAddress)
        {
            char[] networkId = binaryIpAddress.ToCharArray();
            for (int i = Cidr; i < networkId.Length; i++)
            {
                networkId[i] = '0';
            }
            return new string(networkId);
        }

        /// <summary>
        /// Calculates the binary broadcast address through cidr and a given binary IP-Address.
        /// </summary>
        /// <param name="binaryIpAddress"></param>
        /// <returns>string of bits (ex. 11000000101010000110010000001010)</returns>
        private string CalculateBinaryBroadcastAddress(string binaryIpAddress)
        {
            char[] broadcast = binaryIpAddress.ToCharArray();
            for (int i = Cidr; i < broadcast.Length; i++)
            {
                broadcast[i] = '1';
            }
            return new string(broadcast);
        }

        /// <summary>
        /// Calculates the binary subnet address from the given cidr.
        /// </summary>
        /// <param name="cidr"></param>
        /// <returns>string of bits (ex. 11000000101010000110010000001010)</returns>
        public static string CalculateBinarySubnet(int cidr)
        {
            string binarySubnet = "";
            for (int i = 1; i <= 32; i++)
            {
                if (i <= cidr)
                    binarySubnet += "1";
                else
                    binarySubnet += "0";
            }
            return binarySubnet;
        }

        /// <summary>
        /// Calculates all available Hosts between Network-ID and Broadcast-Address.
        /// </summary>
        /// <param name="availableFrom"></param>
        /// <param name="availableTo"></param>
        /// <returns>out string availableFrom, out string availableTo</returns>
        private void CalculateAvailableHostAddressRange(out string availableFrom, out string availableTo)
        {
            byte[] networkBytes = IPAddress.Parse(NetworkId).GetAddressBytes();
            byte[] broadcastMaskBytes = IPAddress.Parse(BroadcastAddress).GetAddressBytes();

            var from = (int)networkBytes[3] + 1;
            var to = (int)broadcastMaskBytes[3] - 1;

            availableFrom = networkBytes[0] + "." + networkBytes[1] + "." + networkBytes[2] + "." + from.ToString();
            availableTo = broadcastMaskBytes[0] + "." + broadcastMaskBytes[1] + "." + broadcastMaskBytes[2] + "." + to.ToString();
        }

        /// <summary>
        /// Converts a given binary Address into an IP-Address string.
        /// </summary>
        /// <param name="binaryAddress"></param>
        /// <returns>string of IP-Address (ex. 192.168.0.0)</returns>
        private string ConvertToIPAddress(string binaryAddress)
        {
            byte[] addressBytes = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                string octetBinary = binaryAddress.Substring(i * 8, 8);
                addressBytes[i] = Convert.ToByte(octetBinary, 2);
            }
            return new IPAddress(addressBytes).ToString();
        }

        /// <summary>
        /// Converts a given IP-Address into a binary string.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>string of bits (ex. 11000000101010000110010000001010)</returns>
        private string ConvertToBinary(string ipAddress)
        {
            var sb = new StringBuilder();

            foreach (var octet in ipAddress.Split("."))
            {
                byte b = (byte)int.Parse((string)octet);
                var bit = Convert.ToString(b, 2);
                bit = bit.PadLeft(8, '0');
                sb.Append(bit);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Writes the given binary address to the console.
        /// All bits in the cidr range are colored blue.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="binaryAddress"></param>
        /// <param name="cidr"></param>
        private void DisplayBinaryAddress(string label, string binaryAddress)
        {
            Console.Write($"{label}");
            Console.Write("[");
            for (int i = 0; i < 32; i++)
            {
                if (i > 0 && i % 8 == 0)
                {
                    Console.ResetColor();
                    Console.Write("].[");
                }
                if (i < Cidr)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(binaryAddress[i]);
                }
                else
                {
                    Console.ResetColor();
                    Console.Write(binaryAddress[i]);
                }
            }
            Console.ResetColor();
            Console.Write("]");
            Console.WriteLine();
        }

        /// <summary>
        /// Writes all calculated information to the Console.
        /// </summary>
        public void ConsoleWrite()
        {
            Console.WriteLine("IP Adresse:\t\t\t" + IpAddress);
            Console.WriteLine("CIDR Notation:\t\t\t" + Cidr);
            Console.WriteLine("Subnetzmaske:\t\t\t" + SubnetAddress);
            Console.WriteLine("Netzwerk-ID:\t\t\t" + NetworkId);
            Console.WriteLine("Broadcast-Adresse:\t\t" + BroadcastAddress);
            Console.WriteLine();
            Console.WriteLine("Insgesamt verfügbare Hosts:\t" + AvailableHosts);
            Console.WriteLine("Erster verfügbarer Host:\t" + availableFrom);
            Console.WriteLine("Letzter verfügbarer Host:\t" + availableTo);
            Console.WriteLine();
            DisplayBinaryAddress("IP-Adresse (Binär):\t\t", BinaryIpAddress);
            DisplayBinaryAddress("Subnetzmaske (Binär):\t\t", BinarySubnetAddress);
            DisplayBinaryAddress("NetzID (Binär):\t\t\t", BinaryNetworkId);
            DisplayBinaryAddress("Broadcast (Binär):\t\t", BinaryBroadcastAddress);
            Console.WriteLine();
        }
    }
}
