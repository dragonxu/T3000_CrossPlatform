﻿namespace T3000.Forms
{
    using PRGReaderLibrary;
    using Properties;
    using System;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public partial class ScreensForm : Form
    {
        public List<ScreenPoint> Points { get; set; }

        public ScreensForm(List<ScreenPoint> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            Points = points;

            InitializeComponent();

            view.ColumnHandles[ModeColumn.Name] =
                DataGridViewUtilities.EditEnumColumn<TextGraphic>;
            view.ColumnHandles[PictureColumn.Name] = EditPictureColumn;

            //Show points
            view.Rows.Clear();
            var i = 0;
            foreach (var point in Points)
            {
                view.Rows.Add(new object[] {
                    i + 1,
                    point.Description,
                    point.Label,
                    point.PictureFile,
                    point.GraphicMode,
                    point.RefreshTime
                });
                ++i;
            }
        }

        #region Buttons

        private void ClearSelectedRow(object sender, EventArgs e)
        {
            var row = view.CurrentRow;

            if (row == null)
            {
                return;
            }

            row.Cells[DescriptionColumn.Name].Value = string.Empty;
            row.Cells[LabelColumn.Name].Value = string.Empty;
            row.Cells[PictureColumn.Name].Value = string.Empty;
            row.Cells[ModeColumn.Name].Value = TextGraphic.Text;
            row.Cells[RefreshColumn.Name].Value = 0;
        }

        private void Save(object sender, EventArgs e)
        {
            try
            {
                var i = 0;
                foreach (DataGridViewRow row in view.Rows)
                {
                    if (i >= Points.Count)
                    {
                        break;
                    }
                    
                    var point = Points[i];
                    point.Description = (string)row.Cells[DescriptionColumn.Name].Value;
                    point.Label = (string)row.Cells[LabelColumn.Name].Value;
                    point.PictureFile = (string)row.Cells[PictureColumn.Name].Value;
                    point.GraphicMode = (TextGraphic)row.Cells[ModeColumn.Name].Value;
                    point.RefreshTime = (int)row.Cells[ModeColumn.Name].Value;
                    ++i;
                }
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

        #region Handles

        private void EditPictureColumn(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = $"{Resources.JpegFiles} (*.jpg)|*.jpg|" +
                                $"{Resources.PngFiles} (*.png)|*.png|" +
                                $"{Resources.AllFiles} (*.*)|*.*";
                dialog.Title = Resources.SelectImageFile;
                dialog.InitialDirectory = Assembly.GetExecutingAssembly().Location;
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var row = view.CurrentRow;
                var cell = row.Cells[PictureColumn.Name];
                cell.Value = dialog.FileName;
                view.EndEdit();
            }
            catch (Exception exception)
            {
                MessageBoxUtilities.ShowException(exception);
            }
        }
        
        #endregion

    }
}
