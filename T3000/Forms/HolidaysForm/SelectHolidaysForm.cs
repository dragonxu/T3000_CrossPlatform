﻿namespace T3000.Forms
{
    using PRGReaderLibrary;
    using System;
    using System.Windows.Forms;

    public partial class SelectHolidaysForm : Form
    {
        public HolidayCode Code { get; set; }

        public SelectHolidaysForm(HolidayCode code)
        {
            InitializeComponent();

            Code = code;

            editTextBox.Text = Code.Code.GetString().ClearBinarySymvols();
        }


        #region Buttons

        private void Save(object sender, EventArgs e)
        {
            try
            {
                Code.Code = editTextBox.Text.ToBytes(2000);
            }
            catch (Exception exception)
            {
                MessageBoxUtilities.ShowException(exception);
                DialogResult = DialogResult.None;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
