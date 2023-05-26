using System.Net;
using System.Text;

namespace NetworkCalculator
{
    public class Network
    {
        private string ipAddress;
        private int cidr;

        private string binaryIpAddress;
        private string binarySubnetAddress;
        private string binaryNetworkId;
        private string binaryBroadcastAddress;

        private string subnetAddress;
        private string networkId;
        private string broadcastAddress;

        private int availableHosts;
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
            ipAddress = ipCidr.Split("/")[0];
            cidr = int.Parse(ipCidr.Split("/")[1]);

            if (ipAddress.Contains("+") || ipCidr.Contains("-"))
                throw new Exception();

            if (cidr < 0 || cidr > 31)
                throw new Exception();

            binaryIpAddress = ConvertToBinary(ipAddress);
            binarySubnetAddress = CalculateBinarySubnet(cidr);
            binaryNetworkId = CalculateBinaryNetworkID(binaryIpAddress);
            binaryBroadcastAddress = CalculateBinaryBroadcastAddress(binaryIpAddress);

            subnetAddress = ConvertToIPAddress(binarySubnetAddress);
            networkId = ConvertToIPAddress(binaryNetworkId);
            broadcastAddress = ConvertToIPAddress(binaryBroadcastAddress);

            availableHosts = CalculateAvailableHosts();
            CalculateAvailableHostAddressRange(out availableFrom, out availableTo);
        }

        public string GetAvailableFrom() { return availableFrom; }
        public string GetAvailableTo() { return availableTo; }
        public int GetAvailableHosts() { return availableHosts; }
        public string GetSubnetAddress() { return subnetAddress; }
        public string GetNetworkId() { return networkId; }
        public string GetBroadcastAddress() { return broadcastAddress; }
        public string GetNetworkID() { return networkId; }
        public string GetIpAddress() { return ipAddress; }
        public int GetCidrNotation() { return cidr; }
        public string GetBinarySubnetAddress() { return binarySubnetAddress; }
        public string GetBinaryIpAddress() { return binaryIpAddress; }
        public string GetBinaryNetworkId() { return binaryNetworkId; }
        public string GetBinaryBroadcastAddress() { return binaryBroadcastAddress; }


        /// <summary>
        /// Calculates all available hosts through cidr formula.
        /// </summary>
        /// <returns>integer of available hosts</returns>
        public int CalculateAvailableHosts()
        {
            return (int)(Math.Pow(2, 32 - cidr) - 2);
        }

        /// <summary>
        /// Calculates the binary network id through cidr and a given binary IP-Address.
        /// </summary>
        /// <param name="binaryIpAddress"></param>
        /// <returns></returns>
        public string CalculateBinaryNetworkID(string binaryIpAddress)
        {
            char[] networkId = binaryIpAddress.ToCharArray();
            for (int i = cidr; i < networkId.Length; i++)
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
        public string CalculateBinaryBroadcastAddress(string binaryIpAddress)
        {
            char[] broadcast = binaryIpAddress.ToCharArray();
            for (int i = cidr; i < broadcast.Length; i++)
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
        public string CalculateBinarySubnet(int cidr)
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
            byte[] networkBytes = IPAddress.Parse(networkId).GetAddressBytes();
            byte[] broadcastMaskBytes = IPAddress.Parse(broadcastAddress).GetAddressBytes();

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
        static string ConvertToIPAddress(string binaryAddress)
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
        public string ConvertToBinary(string ipAddress)
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
        public void DisplayBinaryAddress(string label, string binaryAddress, int cidr)
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
                if (i < cidr)
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
            Console.WriteLine("IP Adresse:\t\t\t" + GetIpAddress());
            Console.WriteLine("CIDR Notation:\t\t\t" + GetCidrNotation());
            Console.WriteLine("Subnetzmaske:\t\t\t" + GetSubnetAddress());
            Console.WriteLine("Netzwerk-ID:\t\t\t" + GetNetworkId());
            Console.WriteLine("Broadcast-Adresse:\t\t" + GetBroadcastAddress());
            Console.WriteLine();
            Console.WriteLine("Insgesamt verfügbare Hosts:\t" + GetAvailableHosts());
            Console.WriteLine("Erster verfügbarer Host:\t" + GetAvailableFrom());
            Console.WriteLine("Letzter verfügbarer Host:\t" + GetAvailableTo());
            Console.WriteLine();
            DisplayBinaryAddress("IP-Adresse (Binär):\t\t", GetBinaryIpAddress(), GetCidrNotation());
            DisplayBinaryAddress("Subnetzmaske (Binär):\t\t", GetBinarySubnetAddress(), GetCidrNotation());
            DisplayBinaryAddress("NetzID (Binär):\t\t\t", GetBinaryNetworkId(), GetCidrNotation());
            DisplayBinaryAddress("Broadcast (Binär):\t\t", GetBinaryBroadcastAddress(), GetCidrNotation());
            Console.WriteLine();
        }
    }
}
