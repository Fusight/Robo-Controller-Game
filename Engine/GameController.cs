﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Engine
{
    public class GameController
    {
        public GameWorld gameWorld;
        public Robot robot;
        public Renderer renderer;
        private int entityCounter = 0;
        private Programm runningProgramm;

        private DispatcherTimer commandTimer;
        private int timeElapsed = 0;
        private ProgressBar CPUprogressBar;

        public List<RobotEquipment> robotEquipment = new List<RobotEquipment>();
        private List<RobotEquipment> initalEquipment;
        public List<RobotEquipment> activeEquipment;
        public List<RobotEquipment> toBuy;

        public bool inAnimation = false;

        public Player player;

        public GameController(int width, int height, Canvas GameBoard, ProgressBar CPUProgressBar, Window window)
        {
            //setup player
            player = new Player();

            CPUprogressBar = CPUProgressBar;
            Point start = new Point(0, 0);
            runningProgramm = new Programm(this);
            gameWorld = new GameWorld(width, height, start);
            renderer = new Renderer(this, GameBoard, gameWorld.map.Cast<GameObject>().ToList());

            commandTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(1000)
            };
            EventHandler eventHandler = new EventHandler(HandleCommandTime);
            commandTimer.Tick += eventHandler;
            commandTimer.Start();

            InitialRobotInfo();
            robot = new Robot(entityCounter++, start, this, activeEquipment);
            //Link robot to equipment
            activeEquipment.ForEach(e => e.robot = robot);
            robotEquipment.ForEach(e => e.robot = robot);
            initalEquipment.ForEach(e => e.robot = robot);

            //Calculate robot values
            robot.equipment.ForEach(e => e.SettupRobot());

            renderer.AddEntity(robot, isImage: true);
            SettupRobotInfo();
        }

        private void InitialRobotInfo()
        {
            #region Setup game state

            {
                robotEquipment.Add(new BasicCPU("BasicCPUMk1", robot));
                robotEquipment.Add(new BasicCasing("BasicCasingMk1", robot));
                robotEquipment.Add(new BasicRAMMK1(robot));
                initalEquipment = new List<RobotEquipment>(robotEquipment);
                activeEquipment = new List<RobotEquipment>(initalEquipment);
                robotEquipment.Add(new BasicDrill(robot));
                robotEquipment.Add(new BasicCasingMK2(robot));
                robotEquipment.Add(new BasicRAMMK2(robot));
                toBuy = new List<RobotEquipment>(robotEquipment.Where(e => activeEquipment.FindIndex(a => a.id == e.id) == -1));
            }

            #endregion Setup game state
        }

        private void SettupRobotInfo()
        {
            CPUprogressBar.Maximum = robot.CPUSpeed;
            return;
        }

        private void HandleCommandTime(object obj, EventArgs args)
        {
            if (runningProgramm.Finished || inAnimation) return;
            timeElapsed += 10;
            string error = "";
            if (timeElapsed >= robot.CPUSpeed)
            {
                timeElapsed = 0;
                runningProgramm.ExecuteLine(out error);
                renderer.Update();
            }
            if (error != "")
            {
                MessageBox.Show(error);
                //Clear Program
                runningProgramm.Clear();
            }
            try
            {
                CPUprogressBar.Value = timeElapsed;
            }
            catch (Exception)
            {
            }
            return;
        }

        public bool RunCode(string code, out string Error)
        {
            Error = "";
            if (!runningProgramm.Finished || inAnimation) return true;
            if (!runningProgramm.Parse(code, out string error))
            {
                Error = "Input is incorrect: " + error;
                return false;
            }
            return true;
        }
    }
}