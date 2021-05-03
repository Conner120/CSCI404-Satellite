using System;
using System.IO;
using System.Text;

namespace OneTimePad
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Ask the user to type in a .txt file name to be worked on.
            Console.WriteLine("Enter a text file. (DO NOT include the .txt extension)");
            var input = Console.ReadLine();

            // The hardcoded file path is what so far works
            // due to how VS Studio runs the program.
            // A more general file path will be added once found.
            var messageFile = new StreamReader("../../../" + input + ".txt").ReadToEnd();

            // Display the .txt file's contents to the user.
            Console.WriteLine("\nBeginning Message:");
            Console.WriteLine(messageFile + "\n");

            // Converting text to bytes, assuming unicode.
            byte[] originalBytes = Encoding.Unicode.GetBytes(messageFile);
            
            

            // generate a pad in memory.
            byte[] pad = GeneratePad(size: originalBytes.Length, seed: 1);
            // The pad is converted to base64.
            var OTP = Convert.ToBase64String(inArray: pad);


            // Encrypt the text file's contents.
            byte[] encrypted = Encrypt(originalBytes, pad);
            // The encrypted message.
            var encryptito = Convert.ToBase64String(inArray: encrypted);

            byte[] encryptedFromBase64 = Convert.FromBase64String(encryptito);

            // Decrypting the encrypted message using the pad.
            byte[] decrypted = Decrypt(encryptedFromBase64, pad);
            var count = 0;
            while (count == 0)
            {
                // Provide the user 3 options of methods to perform on a file.
                Console.WriteLine("Select an Option:\n" +
                "1. encrypt my file\n" +
                "2. decrypt my file\n" +
                "3. all methods\n" +
                "4. exit program\n");
                var methodinput = Console.ReadLine();

                // If the user selects 'encrypt' or '1' the program will encrypt
                // the message and generate an OTP
                if (methodinput == "encrypt" | methodinput == "1")
                {
                    Console.WriteLine("The one time pad.");
                    Console.WriteLine(OTP + "\n");
                    File.WriteAllTextAsync("../../../One Time Pad.txt", OTP);

                    Console.WriteLine("Encrypted Message:");
                    Console.WriteLine(encryptito + "\n");
                    File.WriteAllTextAsync("../../../" + input + "-encrypted.txt", encryptito);
                }
                // If the user selects 'decrypt' or '2' the program will decrypt
                // the message and display it to the user.
                else if (methodinput == "decrypt" | methodinput == "2")
                {
                    Console.WriteLine("The decrypted message.");
                    Console.WriteLine(Encoding.Unicode.GetString(decrypted) + "\n");
                }
                // If the user selects 'all' or '3' the program will execute
                // all functions onto the text file's contents and display
                // the results to the user accordingly.
                else if (methodinput == "all" | methodinput == "3")
                {
                    Console.WriteLine("The one time pad.");
                    Console.WriteLine(OTP + "\n");
                    File.WriteAllTextAsync("../../../One Time Pad.txt", OTP);

                    Console.WriteLine("Encrypted Message:");
                    Console.WriteLine(encryptito + "\n");
                    File.WriteAllTextAsync("../../../" + input + "-encrypted.txt", encryptito);

                    Console.WriteLine("The decrypted message.");
                    Console.WriteLine(Encoding.Unicode.GetString(decrypted) + "\n");
                }
                // If the user selects 'exit' or '4' the program will
                // exit the console environment, causing it to close.
                else if (methodinput == "exit" | methodinput == "4")
                {
                    Environment.Exit(69);
                }
                // Else statement just returns the 4 original options.
                // Meant to handle mistyped actions.
                else{}
            }


        }

        public static byte[] GeneratePad(int size, int seed)
        {
            var random = new Random(Seed: seed);
            var bytesBuffel = new byte[size];

            random.NextBytes(bytesBuffel);

            return bytesBuffel;
        }

        public static byte[] Encrypt(byte[] data, byte[] pad)
        {
            var result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                var sum = (int)data[i] + (int)pad[i];
                if (sum > 255)
                    sum -= 255;
                result[i] = (byte)sum;
            }
            return result;
        }

        public static byte[] Decrypt(byte[] encrypted, byte[] pad)
        {
            var result = new byte[encrypted.Length];
            for (int i = 0; i < encrypted.Length; i++)
            {
                var dif = (int)encrypted[i] - (int)pad[i];
                if (dif < 0)
                    dif += 255;
                result[i] = (byte)dif;
            }
            return result;
        }

    }
}
