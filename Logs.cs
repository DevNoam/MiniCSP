﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _365
{
    public partial class Logs : Form
    {
        public Logs(int _id)
        {
            InitializeComponent();
            id.Text = _id.ToString();
        }
    }
}
