using System;
using System.IO;
using System.Security.Cryptography;

class Program
{
    static string CalculeazaSHA256(string numeFisier)
    {
        SHA256 sha256 = SHA256.Create();
        FileStream fisier = File.OpenRead(numeFisier);
        
        byte[] hash = sha256.ComputeHash(fisier);
        string rezultat = BitConverter.ToString(hash);
        
        fisier.Close();
        sha256.Dispose();
        
        return rezultat.Replace("-", "").ToLower();
    }
    
    static string CalculeazaHMAC(string numeFisier)
    {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] cheie = new byte[64];
        rng.GetBytes(cheie);
        rng.Dispose();
        
        HMACSHA256 hmac = new HMACSHA256(cheie);
        FileStream fisier = File.OpenRead(numeFisier);
        
        byte[] hash = hmac.ComputeHash(fisier);
        string rezultat = BitConverter.ToString(hash);
        
        fisier.Close();
        hmac.Dispose();
        
        return rezultat.Replace("-", "").ToLower();
    }
    
    static void ModificaFisier(string numeFisier)
    {
        byte[] continut = File.ReadAllBytes(numeFisier);
        
        if (continut.Length > 0)
        {
            continut[0] = (byte)(continut[0] + 1);
            File.WriteAllBytes(numeFisier, continut);
        }
    }
    
    static void Main()
    {
        string numeFisier = "fisier_test.bin";
        
        if (!File.Exists(numeFisier))
        {
            File.WriteAllText(numeFisier, "Acesta este un fisier de test.");
        }
        
        Console.WriteLine("Hash-urile originale:");
        string sha256Original = CalculeazaSHA256(numeFisier);
        string hmacOriginal = CalculeazaHMAC(numeFisier);
        
        Console.WriteLine("SHA-256:    " + sha256Original);
        Console.WriteLine("HMAC-SHA256: " + hmacOriginal);
        
        ModificaFisier(numeFisier);
        
        Console.WriteLine("\nHash-urile dupa modificare:");
        string sha256Nou = CalculeazaSHA256(numeFisier);
        string hmacNou = CalculeazaHMAC(numeFisier);
        
        Console.WriteLine("SHA-256:    " + sha256Nou);
        Console.WriteLine("HMAC-SHA256: " + hmacNou);
        
        Console.WriteLine("\nComparatie:");
        Console.WriteLine("SHA-256 schimbat: " + (sha256Original != sha256Nou));
        Console.WriteLine("HMAC schimbat: " + (hmacOriginal != hmacNou));
    }
}