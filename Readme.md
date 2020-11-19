# Serial connection, in C#, to a FONA 3G GSM.

This is a quick test program for the FONA 3G GSM module by Adafruit.
The program opens a serial line to the device, does a quick AT command to see
if it's ok.

Following that, it'll prompt for a telephone number and a line of text, and
attempt to send a text message.

So far, it works from my PC but the next step is to try it from a Raspberry Pi 2.

The program takes the com port or device id as a command line parameter.
It'll prompt for the telephone number and text later.
