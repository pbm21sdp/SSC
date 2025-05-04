using System; // namespace de baza pentru functionalitati fundamentale 
using System.IO; // namespace pentru operatii cu fisiere si stream-uri
using System.Security.Cryptography; // namespace pentru functii criptografice
using System.Text; // namespace pentru manipularea textului si encoding

class HashDemonstrator // clasa principala care demonstreaza functiile hash
{
    static string ComputeSHA256(string filePath)
    {
        // folosim 'using' pentru a asigura eliberarea corecta a resurselor
        using (SHA256 sha256 = SHA256.Create()) // creeaza o instanta a algoritmului SHA-256
        using (FileStream stream = File.OpenRead(filePath)) // deschide fisierul pentru citire in mod stream
        {
            byte[] hash = sha256.ComputeHash(stream); // calculeaza hash-ul pentru intregul continut al fisierului
            // converteste array-ul de bytes in string hexazecimal
            // BitConverter.ToString() creeaza un string cu bytes separati prin cratime 
            // Replace("-", "") elimina cratimele dintre bytes
            // ToLower() converteste toate literele in litere mici pentru consistenta
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    static string ComputeHMACSHA256(string filePath, byte[] key)
    {
        using (HMACSHA256 hmac = new HMACSHA256(key)) // creeaza o instanta HMACSHA256 cu cheia specificata
        using (FileStream stream = File.OpenRead(filePath)) // deschide fisierul pentru citire in mod stream
        {
            byte[] hash = hmac.ComputeHash(stream); // calculeaza HMAC pentru continutul fisierului
            // converteste rezultatul in string hexazecimal, la fel ca la SHA256
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }

    static void ModifyFile(string filePath)
    {
        // deschide fisierul cu permisiuni de citire si scriere
        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
        {
            // verifica daca fisierul este gol pentru a evita erori
            if (stream.Length == 0)
                throw new InvalidOperationException("Fișierul este gol");

            // schimba primul byte
            stream.Position = 0; // seteaza pozitia de citire la inceputul fisierului
            byte originalByte = (byte)stream.ReadByte(); // citeste primul byte si il converteste la byte
            stream.Position = 0; // reseteaza pozitia la inceput pentru scriere
            // scrie primul byte incrementat cu 1 (demonstreaza o modificare minima)
            stream.WriteByte((byte)(originalByte + 1));
        }
    }

    static void Main(string[] args)
    {
        // defineste numele fisierului de test
        const string filePath = "fisier_test.bin";
        byte[] hmacKey; // declara variabila pentru cheia HMAC

        // genereaza o cheie HMAC aleatorie si sigura
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) // creeaza un generator de numere aleatoare criptografic
        {
            hmacKey = new byte[64]; // aloca 64 bytes (512 biti) pentru cheie
            rng.GetBytes(hmacKey); // umple array-ul cu bytes aleatori
        }

        try // bloc try pentru gestionarea erorilor
        {
            // hash original
            Console.WriteLine("Hash original:"); 
            Console.WriteLine($"SHA-256:    {ComputeSHA256(filePath)}"); // calculeaza si afiseaza SHA-256
            Console.WriteLine($"HMAC-SHA256: {ComputeHMACSHA256(filePath, hmacKey)}"); // calculeaza si afiseaza HMAC-SHA256
            
            // modifică fisierul
            ModifyFile(filePath); // apeleaza functia pentru a modifica primul byte al fisierului
            Console.WriteLine("\nFisier modificat...\n"); // informeaza utilizatorul ca fisierul a fost modificat

            // hash dupa modificare
            Console.WriteLine("Hash dupa modificare:"); 
            Console.WriteLine($"SHA-256:    {ComputeSHA256(filePath)}"); // recalculeaza si afiseaza SHA-256
            Console.WriteLine($"HMAC-SHA256: {ComputeHMACSHA256(filePath, hmacKey)}"); // recalculeaza si afiseaza HMAC-SHA256
        }
        catch (Exception ex) // prinde orice exceptie care ar putea aparea
        {
            Console.WriteLine($"Eroare: {ex.Message}"); // afiseaza mesajul de eroare
        }
    }
}