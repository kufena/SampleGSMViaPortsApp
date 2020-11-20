using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace SampleGSMViaPortsApp
{
    /**
     * This program uses a serial connection to connect to a FONA 3G GSM board.
     * The connection uses AT commands, which are a bit sensitive to completing.
     * 
     * Starts by taking in a com port number or device id of some sort.
     * 
     * It prompts for a telephone number and a simple single line message.
     * 
     * This simple program starts by send the basic AT<cr> command and looking
     * for an OK in the last line of the response.  For some reason, mine also
     * repeats some of the commands - it does here, for instance.
     * An OK not found means we terminate.
     * 
     * Then it uses the AT+CMGS to send the text message to the number given,
     * again, looking for some prompts, although it doesn't always get anything
     * back as the program is too quick.
     * 
     */
    class Program
    {
        static void Main(string[] args)
        {
            // Some welcome stuff.
            // We expect device to be given as cmd line argument.
            Console.WriteLine("Simple App to send an sms");
            Console.WriteLine($"Using device {args[0]}");
            int baud = 115200;
            if (args.Length > 1)
            {
                if (!Int32.TryParse(args[1], out baud))
                {
                    Console.WriteLine($"Value \"{args[1]}\" is not an integer.");
                    return;
                }
            }

            bool dtrEnabled = true;
            if (args.Length > 2)
            {
                Boolean.TryParse(args[2], out dtrEnabled);
            }

            // Open the serial port to our device and ensure it is in an
            // open state.  It'll throw an exception if there's some other
            // problem like if already in use (is Putty still connected?)
            SerialPort sp = new SerialPort(args[0], baud); //, 115200);
            sp.DtrEnable = dtrEnabled;
            sp.Open();
            Console.WriteLine($"baud is {sp.BaudRate}");
            if (!sp.IsOpen)
            {
                Console.WriteLine("Not open for some reason.");
                return;
            }

            // Send the most basic AT command and hope.
            // I've converted to ascii here, but there seems to be no need.
            byte[] atcmd = ASCIIEncoding.ASCII.GetBytes("AT\r" );
            Console.WriteLine($"length of cmd is {atcmd.Length}");
            sp.Write(atcmd, 0, 3);

            string back = "";
            while (sp.BytesToRead > 0)
            {
                back = sp.ReadLine();
                Console.WriteLine(back);
            }

            if (back.Contains("ERROR"))
            {
                Console.WriteLine("whoops!");
                return;
            }

            // Get number and line of text.
            Console.WriteLine("Enter the number to send to:");
            string number = Console.ReadLine();
            Console.WriteLine("Enter message as one line:");
            string msg = Console.ReadLine();

            // Send the commands for the text message.
            // I found this a bit difficult at times.
            // If you have a problem, kill the program and connect a serial terminal to the
            // device - like Putty - and press return a few times.  If you see some '>' prompts,
            // then likely it is mid command.
            // I found pressing ctrl-z then <ESC> pulled it out of the command.
            // Then type AT and return and see if an OK is returned.
            // If not reset the device, i guess.
            //
            // NOTE \u001a is unicode for ctrl-z, \u001B is the escape charater.
            sp.WriteLine($"AT+CMGS=\"{number}\"\r");
            sp.WriteLine($"{msg}\r\u001a\u001B"); // sendmsg, 0, sendmsg.Length);

            // Let's sleep for 5 minutes to see if there's some text back.
            Thread.Sleep(5000);

            // Read the text back.
            while (sp.BytesToRead > 0)
            {
                back = sp.ReadLine();
                Console.WriteLine(back);
            }

            Console.WriteLine($"The actual last line was \"{back}\"");
            if (back.Contains("OK"))
            {
                Console.WriteLine("Expect a message any time!");
            }

            if (back.Contains("ERROR"))
            {
                Console.WriteLine("whoops!");
            }

            // That's it.
            Console.WriteLine("Hit return.");
            Console.ReadLine();

        }
    }
}
