# String to WAV BFSK

## Description
Securely transmit digital text over analog radio signals. For demonstration purposes, a WAV file with the modulated audio is generated in the desktop. The string is encrypted using AES-256 with a pseudo-randomly selected key.\

## How it works
1. The list of encryption keys is loaded from the `keys.txt` file in the host's desktop folder
2. The user enters the message they wish to transmit
3. A random key is selected from the list
4. The message is encrypted with the key and a random IV (`System.Security.Cryptography` native library is used)
5. The number of the key used is appended to the encrypted string for the receiver
6. Binary FSK modulation is used to generate a WAV file with the modulated signal based on a specified carrier

I'm aware of how useless the program is. It's just a proof of concept.