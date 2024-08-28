using System;
using System.Collections.Generic;
using System.IO;

namespace LeitorDeVCF
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Por favor, insira o caminho do arquivo VCF:");
            string l_filePath = Console.ReadLine();

            if (!File.Exists(l_filePath))
            {
                Console.WriteLine("Arquivo não encontrado. Certifique-se de que o caminho está correto.");
                return;
            }

            var l_contacts = ProcessVcf(l_filePath);

            // Processar os contatos, por exemplo, imprimir na tela
            foreach (var contact in l_contacts)
            {
                Console.WriteLine($"Nome: {contact.ContactName} {contact.ContactSurname}");
                Console.WriteLine($"Telefone: {contact.Telephone}");
                Console.WriteLine($"Email: {contact.ContactEmail}");
                Console.WriteLine($"Endereço: {contact.PhysicalAddresses}");
                Console.WriteLine();
            }

            Console.ReadKey();
        }

        public static List<FileContatosInfo> ProcessVcf(string p_file)
        {
            var l_contactsInfoList = new List<FileContatosInfo>();

            var l_contacts = File.ReadAllLines(p_file);
            FileContatosInfo l_currentContact = null;

            foreach (var l_contact in l_contacts)
            {
                if (l_contact.StartsWith("BEGIN:VCARD"))
                {
                    l_currentContact = new FileContatosInfo();
                }
                else if (l_contact.StartsWith("FN:"))
                {
                    l_currentContact.ContactName = l_contact.Substring(3).Trim();
                }
                else if (l_contact.StartsWith("N:"))
                {
                    var l_nameParts = l_contact.Substring(2).Split(';');
                    l_currentContact.ContactSurname = l_nameParts.Length > 0 ? l_nameParts[0] : null;
                }
                else if (l_contact.StartsWith("TEL"))
                {
                    var l_telValue = l_contact.Split(':')[1].Trim(); // Pega o número do telefone após "TEL;TYPE=..."
                    if (string.IsNullOrEmpty(l_currentContact.Telephone))
                    {
                        l_currentContact.Telephone = l_telValue;
                    }
                    else
                    {
                        l_currentContact.Telephone += $"; {l_telValue}";
                    }
                }
                else if (l_contact.StartsWith("EMAIL"))
                {
                    var l_emailValue = l_contact.Split(':')[1].Trim(); // Pega o e-mail após "EMAIL;TYPE=..."
                    if (string.IsNullOrEmpty(l_currentContact.ContactEmail))
                    {
                        l_currentContact.ContactEmail = l_emailValue;
                    }
                    else
                    {
                        l_currentContact.ContactEmail += $"; {l_emailValue}";
                    }
                }
                else if (l_contact.StartsWith("ADR"))
                {
                    var l_addressValue = l_contact.Split(':')[1].Trim(); // Pega o endereço após "ADR;TYPE=..."
                    if (string.IsNullOrEmpty(l_currentContact.PhysicalAddresses))
                    {
                        l_currentContact.PhysicalAddresses = l_addressValue.Replace(";", ", ");
                    }
                    else
                    {
                        l_currentContact.PhysicalAddresses += $"; {l_addressValue.Replace(";", ", ")}";
                    }
                }
                else if (l_contact.StartsWith("END:VCARD"))
                {
                    l_contactsInfoList.Add(l_currentContact);
                }
            }
            return l_contactsInfoList;
        }
    }
}
