﻿namespace T3000.Forms
{
    using PRGReaderLibrary;
    using Properties;
    using System;
    using System.IO;
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

            //User input handles
            view.AddEditHandler(ModeColumn, TViewUtilities.EditEnum<TextGraphic>);
            view.AddEditHandler(PictureColumn, EditPictureColumn);
            view.AddEditHandler(ScreenColumn, EditScreenColumn);

            //Value changed handles
            view.AddChangedHandler(ModeColumn, TViewUtilities.ChangeEnabled,
                PictureColumn, ModeColumn);

            //Show points
            view.Rows.Clear();
            view.Rows.Add(Points.Count);
            for (var i = 0; i < Points.Count; ++i)
            {
                var point = Points[i];
                var row = view.Rows[i];

                //Read only
                row.SetValue(NumberColumn, $"SCR{i + 1}");

                SetRow(row, point);
            }

            //Validation
            view.AddValidation(DescriptionColumn, TViewUtilities.ValidateString, 21);
            view.AddValidation(LabelColumn, TViewUtilities.ValidateString, 9);
            view.AddValidation(RefreshColumn, TViewUtilities.ValidateInteger);
            view.AddValidation(CountColumn, TViewUtilities.ValidateInteger);
            view.Validate();
        }

        private void SetRow(DataGridViewRow row, ScreenPoint point)
        {
            if (row == null || point == null)
            {
                return;
            }

            row.SetValue(DescriptionColumn, point.Description);
            row.SetValue(LabelColumn, point.Label);
            row.SetValue(PictureColumn, point.PictureFile);
            row.SetValue(ModeColumn, point.GraphicMode);
            row.SetValue(RefreshColumn, point.RefreshTime);
            row.SetValue(CountColumn, 0);
        }

        #region Buttons

        private void ClearSelectedRow(object sender, EventArgs e) =>
            SetRow(view.CurrentRow, new ScreenPoint());

        private void Save(object sender, EventArgs e)
        {
            if (!view.Validate())
            {
                MessageBoxUtilities.ShowWarning(Resources.ViewNotValidated);
                DialogResult = DialogResult.None;
                return;
            }

            try
            {
                for (var i = 0; i < view.RowCount && i < Points.Count; ++i)
                {
                    var point = Points[i];
                    var row = view.Rows[i];
                    point.Description = row.GetValue<string>(DescriptionColumn);
                    point.PictureFile = row.GetValue<string>(PictureColumn);
                    point.GraphicMode = row.GetValue<TextGraphic>(ModeColumn);
                    point.Label = row.GetValue<string>(LabelColumn);
                    point.RefreshTime = row.GetValue<int>(RefreshColumn);
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

        #region User input handles

        private void EditScreenColumn(object sender, EventArgs e)
        {
            try
            {
                var name = view.CurrentRow.GetValue<string>(PictureColumn);
                var building = "Default_Building";
                var path = GetFullPathForPicture(name, building);

                var form = new EditScreenForm(path);
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }
            catch (Exception exception)
            {
                MessageBoxUtilities.ShowException(exception);
            }
        }

        private string GetFullPathForPicture(string name, string building) =>
            Path.Combine("Database", "Buildings", building, "image", name);

        private void EditPictureColumn(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = $"{Resources.ImageFiles} (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png|" +
                                $"{Resources.AllFiles} (*.*)|*.*";
                dialog.Title = Resources.SelectImageFile;
                dialog.InitialDirectory = Assembly.GetExecutingAssembly().Location;
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var building = "Default_Building";
                var name = Path.GetFileName(dialog.FileName);
                var path = GetFullPathForPicture(name, building);

                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.Copy(dialog.FileName, path, overwrite: true);

                var row = view.CurrentRow;
                var cell = row.Cells[PictureColumn.Name];
                cell.Value = name;
            }
            catch (Exception exception)
            {
                MessageBoxUtilities.ShowException(exception);
            }
        }
        
        #endregion

    }
}
