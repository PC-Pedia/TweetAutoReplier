using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TweetAutoReplier.Models;

namespace TweetAutoReplier.FileHandlers
{
    public static class FollowersFile
    {
        private static string _filePath = ".\\followers.json";

        public static List<Follower> OpenCollectionFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Followers file|*.json";
            openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            return openFileDialog.ShowDialog() == true ? ReadFromFile(openFileDialog.FileName) : null;
        }

        public static List<Follower> LoadCollectionFromFile()
        {
            return File.Exists(_filePath) == true ? ReadFromFile(_filePath) : null;
        }

        public static void WriteCollectionToFile(ObservableCollection<Follower> collection)
        {
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(collection.ToArray()));
        }

        private static List<Follower> ReadFromFile(string filePath)
        {
            return JsonConvert.DeserializeObject<List<Follower>>(File.ReadAllText(filePath));
        }
    }
}
