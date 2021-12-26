using Bezao.Layer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Bezao.Data
{
    public class DataLayer
    {
        //Appends an object to File in Json format(create)
        public void AddToFile<T>(T obj)
        {
            string jsonOutPut = JsonSerializer.Serialize(obj);
            if (obj is Admin)
            {
                File.AppendAllText("admins.txt", jsonOutPut + Environment.NewLine);
            }
            else if (obj is Customer)
            {
                File.AppendAllText("customers.txt", jsonOutPut + Environment.NewLine);
            }
            else if (obj is Transaction)
            {
                File.AppendAllText("transaction.txt", jsonOutPut + Environment.NewLine);
            }
        }

        //Clears Last Data and Save new List to file in json format(uow)
        public void SaveToFile<T>(List<T> list)
        {
            // Overwrite the file with first object in the list
            string jsonOutPut = JsonSerializer.Serialize(list[0]);
            if (list[0] is Admin)
            {
                File.WriteAllText("admins.txt", jsonOutPut + Environment.NewLine);
            }
            else if (list[0] is Customer)
            {
                File.WriteAllText("customers.txt", jsonOutPut + Environment.NewLine);
            }

            //Appends the other objects of list to the file
            for (int i = 1; i < list.Count; i++)
            {
                AddToFile(list[i]);
            }
        }

        //Returns a list of objects from file(get)
        public List<T> ReadFile<T>(string FileName)
        {
            List<T> list = new List<T>();
            string FilePath = Path.Combine(Environment.CurrentDirectory, FileName);
            StreamReader sr = new StreamReader(FilePath);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                list.Add(JsonSerializer.Deserialize<T>(line));
            }
            sr.Close();
            return list;

        }

        //Delete a customer object from file
        public void DeleteFromFile(Customer customer)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");
            // checking if the customer exist and then remove the required object from the list
            foreach (Customer item in list)
            {
                if (item.AccountNo == customer.AccountNo)
                {
                    list.Remove(item);
                    break;
                }
            }
            //Overwriting the list to file
            SaveToFile<Customer>(list);
        }

        //updates a customer object in the file(put)
        public void UpdateInFile(Customer customer)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].AccountNo == customer.AccountNo)
                {
                    list[i] = customer;
                }
            }
            //Overwriting the list to file
            SaveToFile<Customer>(list);
        }

        //checks if an Admin object is in the File
        public bool IsInFile(Admin admin)
        {
            List<Admin> list = ReadFile<Admin>("admins.txt");
            foreach (Admin user in list)
            {
                if (user.Username == admin.Username && user.Pin== admin.Pin)
                {
                    return true;
                }

            }
            return false;
        }

        //checks if a customer objet is in the file
        public int IsUserActive(string user)
        {
            List<Customer> list = ReadFile<Customer>("customer.txt");
            foreach (Customer customer in list)
            {
                if (customer.Username == user && customer.Status == "Active")
                {
                    return 1;
                }

                else if (customer.Username == user && customer.Status == "Disabled")
                {
                    return 2;
                }

            }
            return 0;
        }
        //checks if a user has the access to Login

        public bool CanLogIn(Customer customer)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");
            foreach (Customer user in list)
            {
                if (customer.Username == user.Username && customer.Pin == user.Pin && user.Status == "Active")
                {
                    return true;
                }
            }
            return false;
        }

        //checks if an account number is in File
        public bool IsInFile(int accNo, out Customer Outcustomer)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");
            foreach (Customer customer in list)
            {
                if (customer.AccountNo == accNo)
                {
                    Outcustomer = customer;
                    return true;
                }
            }
            Outcustomer = null;
            return false;
        }

        //check if a username is in file
        public bool IsInFile(string username)
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");
            foreach (Customer customer in list)
            {
                if (customer.Username == username)
                {
                    return true;
                }
            }
            return false;
        }

        //Returns an object if given userName

        public Customer GetCustomer(string username)
        {
            List<Customer> list = ReadFile<Customer>("customer.txt");
            foreach (Customer customer in list)
            {
                if (customer.Username == username)
                {
                    return customer;
                }
            }
            return null;
        }

        // Returns an object if given account number
        public Customer GetCustomer(int accNo)
        {
            List<Customer> list = ReadFile<Customer>("customer.txt");
            foreach (Customer customer in list)
            {
                if (customer.AccountNo == accNo)
                {
                    return customer;
                }
            }
            return null;
        }

        //Method to get the last account number
        public int GetLastAccountNumber()
        {
            List<Customer> list = ReadFile<Customer>("customers.txt");
            if (list.Count > 0)
            {
                Customer customer = list[list.Count - 1];
                return customer.AccountNo;
            }
            return 0;
        }

        //Deduct amount from balance of an account and update it in file
        public void DeductBalance(Customer customer, int amount)
        {
            customer.Balance -= amount;
            UpdateInFile(customer);
        }

        //Add amount to balance of an account and update it in file
        public void AddAmount(Customer customer, int amount)
        {
            customer.Balance += amount;
            UpdateInFile(customer);
        }

        // Returns total amount a customer has withdrawn today
        public int TodaysTransactionsAmount(int accNo)
        {
            List<Transaction> list = ReadFile<Transaction>("transactions.txt");
            int totalAmount = 0;
            //Checking the transactions and adding the amount
            foreach (Transaction transaction in list)
            {
                if (transaction.AccountNo == accNo)
                {
                    totalAmount += transaction.TransactionAmount;
                }
            }
            return totalAmount;
        }
    }
}
