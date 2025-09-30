using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlandoDigitalImagerProcessor
{
    public partial class Form2 : Form
    {
        public enum EmbossType
        {
            None,
            Laplacian,
            Vertical,
            Horizontal,
            Lossy,
            AllDirections,
            HorzVertical
        }

        public EmbossType SelectedEmboss { get; private set; } = EmbossType.None;

        public Form2()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void onLaplacianClicked(object sender, EventArgs e)
        {
            SelectedEmboss = EmbossType.Laplacian;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void onVerticalClicked(object sender, EventArgs e)
        {
            SelectedEmboss = EmbossType.Vertical;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void onHorizontalClicked(object sender, EventArgs e)
        {
            SelectedEmboss = EmbossType.Horizontal;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void onLossyClicked(object sender, EventArgs e)
        {
            SelectedEmboss = EmbossType.Lossy;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void onAllDirectionsClicked(object sender, EventArgs e)
        {
            SelectedEmboss = EmbossType.AllDirections;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void onHorizontalVerticalClicked(object sender, EventArgs e)
        {
            SelectedEmboss = EmbossType.HorzVertical;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void onCancelClicked(object sender, EventArgs e)
        {
            SelectedEmboss = EmbossType.None;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
