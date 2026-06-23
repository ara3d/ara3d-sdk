
using System.Windows;
using System.Windows.Threading;
using Ara3D.BimOpenSchema;
using Ara3D.Geometry;
using Ara3D.Models;
using Ara3D.Studio.API;

namespace Ara3D.Studio.Tools
{
    public record RoomData(
        string Name,
        int EntityIndex,
        Bounds3D Bounds)
    {
        public Vector3 Center 
            => Bounds.Center;
        
        public RoomData AddBounds(Bounds3D bounds)
            => this with { Bounds = this.Bounds.Include(bounds) };
    }

    public class RoomNavigator 
    {
        public Dictionary<int, RoomData> RoomLookup = [];
        public List<RoomData> Rooms = [];
        public RenderModelData Model;
        public BimData BimData;
        public BimObjectModel BimObjectModel;
        public IHostApplication App;
        public DispatcherTimer Timer;

        public void Execute(Window mainWindow, RenderModelData model, BimData data, IHostApplication app)
        {
            OverlayWindow.Create(mainWindow);

            RoomLookup.Clear();
            Rooms.Clear();
            BimData = data;
            if (BimData == null)
                return;
            Model = model;
            App = app;
            BimObjectModel = new BimObjectModel(BimData, data.Geometry.ToModel3D(), true);

            for (var i = 0; i < model.InstanceCount; i++)
            {
                var inst = model.InstanceData[i];
                var ei = inst.EntityIndex;
                var cat = data.GetCategoryName((EntityIndex)ei);
                if (cat.StartsWith("room", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (inst.MeshIndex < 0) continue;
                    var bounds = model.InstanceBoundsData[i];
                    if (!RoomLookup.ContainsKey(ei))
                    {
                        var name = data.EntityName((EntityIndex)ei);
                        var room = new RoomData(name, ei, bounds);
                        RoomLookup.Add(ei, room);
                    }
                    else
                    {
                        var room = RoomLookup[ei];
                        var newRoom = room.AddBounds(bounds);
                        RoomLookup[ei] = newRoom;
                    }
                }

                Rooms = RoomLookup.Values.ToList();
            }

            DisplayRoomNameWindow();
        }

        public void DisplayRoomNameWindow()
        {
            var form = new Form
            {
                Text = "Room Navigator",
                Width = 400,
                Height = 600,
                StartPosition = FormStartPosition.CenterScreen
            };

            var listBox = new ListBox
            {
                Dock = DockStyle.Fill
            };

            listBox.Items.AddRange(Rooms.Select(r => (object)r.Name).ToArray());

            listBox.DoubleClick += (sender, args) =>
            {
                var index = listBox.SelectedIndex;
                if (index < 0 || index >= RoomLookup.Count)
                    return;
                OnRoomClicked(Rooms[index]);
            };

            form.Controls.Add(listBox);
            form.Show();

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(0.25);
            Timer.Tick += TimerOnTick;
            Timer.IsEnabled = true;
        }

        private void TimerOnTick(object? sender, EventArgs e)
        {
            var state = App.GetCameraState();
            var yaw = state.Yaw.Degrees.Value;
            var pitch = state.Pitch.Degrees.Value;
            var x = state.Position.X;
            var y = state.Position.Y;
            var z = state.Position.Z;
            //var text = $"Yaw = {yaw:N0}, Pitch = {pitch:N0}, X = {x:N2}, Y = {y:N2}, Z = {z:N2}";
            var text = "No room";
            var pt = state.Position;
            foreach (var roomData in Rooms)
            {
                if (roomData.Bounds.Contains(pt))
                {
                    text = $"In room: {roomData.Name}";
                    break;
                }
            }
            OverlayWindow.Instance?.SetText(text);
        }

        private void OnRoomClicked(RoomData room)
        {
            var state = App.GetCameraState();
            App.AnimateCameraTo(state.WithPosition(room.Center), 1.5f);
        }
    }
}
