using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SafetySharp.CaseStudies.Visualizations
{

    using System;
    using System.Windows;
    using System.Windows.Media.Animation;
    using CaseStudies.PressureTank.Modeling;
    using Infrastructure;
    using Modeling;

    public partial class HI_Cell
    {
        private readonly Storyboard _pressureLevelStoryboard;
        private readonly Storyboard _movingStoryboard;
        private readonly Storyboard _sensorAlertStoryboard;
        private readonly Storyboard _cameraAlertStoryboard;
        private Model _model;

        public HI_Cell()
        {
            InitializeComponent();

            // Initialize visualization resources
            _movingStoryboard = (Storyboard)Resources["MovingRobot"];
            _movingStoryboard.Begin();

            _pressureLevelStoryboard = (Storyboard)Resources["PressureLevel"];
            _pressureLevelStoryboard.Begin();
            _pressureLevelStoryboard.Pause();

            _cameraAlertStoryboard = (Storyboard)Resources["CameraEvent"];
            _sensorAlertStoryboard = (Storyboard)Resources["SensorEvent"];
        }


    }
}
