﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class LabelTree : ObservableObject
    {
        private Core.DbModels.LabelDb label;
        public Core.DbModels.LabelDb Label
        {
            get => label;
            set => SetProperty(ref label, value);
        }

        private ObservableCollection<LabelTree> children;
        public ObservableCollection<LabelTree> Children
        {
            get => children;
            set => SetProperty(ref children, value);
        }

        public LabelTree() { }
        public LabelTree(Core.DbModels.LabelDb labelDb) 
        { 
            label = labelDb;
            Name = labelDb.Name;
            Img = labelDb.Img;
            children = new ObservableCollection<LabelTree>();
        }


        public string Name;
        public string Img;

    }
}
