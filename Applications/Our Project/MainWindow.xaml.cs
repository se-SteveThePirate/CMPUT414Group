﻿/*
 * Based on HDFaceBasics from the KinectSDK v2.0 from Microsoft
 * http://www.microsoft.com/en-us/download/details.aspx?id=44561 
 * Copyright (c) Microsoft Corporation.  All rights reserved.
 * 
 * Thomas Cline
 */

namespace TeamBest.KinectKapture
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Face;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Win32;

    /// <summary>
    /// Main Window
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Currently used KinectSensor
        /// </summary>
        private KinectSensor sensor = null;

        /// <summary>
        /// Body frame source to get a BodyFrameReader
        /// </summary>
        private BodyFrameSource bodySource = null;

        /// <summary>
        /// Body frame reader to get body frames
        /// </summary>
        private BodyFrameReader bodyReader = null;

        /// <summary>
        /// HighDefinitionFaceFrameSource to get a reader from.
        /// Also to set the currently tracked user id to get High Definition Face Frames of
        /// </summary>
        private HighDefinitionFaceFrameSource highDefinitionFaceFrameSource = null;

       /// <summary>
        /// HighDefinitionFaceFrameReader to read HighDefinitionFaceFrame to get FaceAlignment
        /// </summary>
        private HighDefinitionFaceFrameReader highDefinitionFaceFrameReader = null;

        /// <summary>
        /// FaceAlignment is the result of tracking a face, it has face animations location and orientation
        /// </summary>
        private FaceAlignment currentFaceAlignment = null;

        /// <summary>
        /// FaceModel is a result of capturing a face
        /// </summary>
        private FaceModel currentFaceModel = null;
        
        /// <summary>
        /// The currently tracked body
        /// </summary>
        private Body currentTrackedBody = null;

        /// <summary>
        /// The currently tracked body
        /// </summary>
        private ulong currentTrackingId = 0;

        /// <summary>
        /// Gets or sets the current tracked user id
        /// </summary>
        private string currentBuilderStatus = string.Empty;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        private string statusText = "Ready To Start Capture";
        /// <summary>
        /// Inidicates that the program is recording
        /// </summary>
        private Boolean recording = false;
        /// <summary>
        /// Used to count the number of frames in the current recording
        /// </summary>
        private int frameCount = 0;
        /// <summary>
        /// List the HighDetailFacePoints that the user has set to record
        /// </summary>
        private List<HighDetailFacePoints> recordPoints = null;
        /// <summary>
        /// Used to output a file containing the capture information
        /// </summary>
        private StreamWriter fileWriter = null;
        /// <summary>
        /// Head point used to relativize the point capture data from the face
        /// </summary>
        private Joint head;

        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the current tracked user id
        /// </summary>
        private ulong CurrentTrackingId
        {
            get
            {
                return this.currentTrackingId;
            }

            set
            {
                this.currentTrackingId = value;

                this.StatusText = this.MakeStatusText();
            }
        }

        /// <summary>
        /// Gets or sets the current Face Builder instructions to user
        /// </summary>
        private string CurrentBuilderStatus
        {
            get
            {
                return this.currentBuilderStatus;
            }

            set
            {
                this.currentBuilderStatus = value;

                this.StatusText = this.MakeStatusText();
            }
        }

        /// <summary>
        /// Called when disposed of
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose based on whether or not managed or native resources should be freed
        /// </summary>
        /// <param name="disposing">Set to true to free both native and managed resources, false otherwise</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.currentFaceModel != null)
                {
                    this.currentFaceModel.Dispose();
                    this.currentFaceModel = null;
                }
            }
        }
        
        /// <summary>
        /// Returns the length of a vector from origin
        /// </summary>
        /// <param name="point">Point in space to find it's distance from origin</param>
        /// <returns>Distance from origin</returns>
        private static double VectorLength(CameraSpacePoint point)
        {
            var result = Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2);

            result = Math.Sqrt(result);

            return result;
        }
        
        /// <summary>
        /// Finds the closest body from the sensor if any
        /// </summary>
        /// <param name="bodyFrame">A body frame</param>
        /// <returns>Closest body, null of none</returns>
        private static Body FindClosestBody(BodyFrame bodyFrame)
        {
            Body result = null;
            double closestBodyDistance = double.MaxValue;

            Body[] bodies = new Body[bodyFrame.BodyCount];
            bodyFrame.GetAndRefreshBodyData(bodies);

            foreach (var body in bodies)
            {
                if (body.IsTracked)
                {
                    var currentLocation = body.Joints[JointType.SpineBase].Position;

                    var currentDistance = VectorLength(currentLocation);

                    if (result == null || currentDistance < closestBodyDistance)
                    {
                        result = body;
                        closestBodyDistance = currentDistance;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Find if there is a body tracked with the given trackingId
        /// </summary>
        /// <param name="bodyFrame">A body frame</param>
        /// <param name="trackingId">The tracking Id</param>
        /// <returns>The body object, null of none</returns>
        private static Body FindBodyWithTrackingId(BodyFrame bodyFrame, ulong trackingId)
        {
            Body result = null;

            Body[] bodies = new Body[bodyFrame.BodyCount];
            bodyFrame.GetAndRefreshBodyData(bodies);

            foreach (var body in bodies)
            {
                if (body.IsTracked)
                {
                    if (body.TrackingId == trackingId)
                    {
                        result = body;
                        break;
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// Helper function to format a status message
        /// </summary>
        /// <returns>Status text</returns>
        private string MakeStatusText()
        {
            string status = "";
            if (this.currentTrackingId != 0)
            {
               status = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Builder Status: {0}, Current Tracking ID: {1}", this.CurrentBuilderStatus, this.CurrentTrackingId);
            }
            else
            {
                status = "Error! No target found!";
            }

            return status;
        }

        /// <summary>
        /// Fires when Window is Loaded
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitializeHDFace();
            foreach (string HDFP in Enum.GetNames(typeof(HighDetailFacePoints)))
                    {
                ListViewItem item = new ListViewItem();
        
                CheckBox facePointItem = new CheckBox();
                facePointItem.IsChecked = true;
                facePointItem.Content=HDFP;
                
                this.FacePointCheckList.Items.Add(facePointItem);    
            }
        
        }

        /// <summary>
        /// Initialize Kinect object
        /// </summary>
        private void InitializeHDFace()
        {

            this.CurrentBuilderStatus = "Ready To Start Capture";

            this.sensor = KinectSensor.GetDefault();
            this.bodySource = this.sensor.BodyFrameSource;
            this.bodyReader = this.bodySource.OpenReader();
            this.bodyReader.FrameArrived += this.BodyReader_FrameArrived;

            this.highDefinitionFaceFrameSource = new HighDefinitionFaceFrameSource(this.sensor);

            this.highDefinitionFaceFrameReader = this.highDefinitionFaceFrameSource.OpenReader();
            this.highDefinitionFaceFrameReader.FrameArrived += this.HdFaceReader_FrameArrived;

            this.currentFaceModel = new FaceModel();
            this.currentFaceAlignment = new FaceAlignment();

            this.InitializeMesh();
            this.UpdateMesh();

            this.sensor.Open();
        }

        /// <summary>
        /// Initializes a 3D mesh to deform every frame
        /// </summary>
        private void InitializeMesh()
        {
            var vertices = this.currentFaceModel.CalculateVerticesForAlignment(this.currentFaceAlignment);

            var triangleIndices = this.currentFaceModel.TriangleIndices;

            var indices = new Int32Collection(triangleIndices.Count);

            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                uint index01 = triangleIndices[i];
                uint index02 = triangleIndices[i + 1];
                uint index03 = triangleIndices[i + 2];

                indices.Add((int)index03);
                indices.Add((int)index02);
                indices.Add((int)index01);
            }

            this.theGeometry.TriangleIndices = indices;
            this.theGeometry.Normals = null;
            this.theGeometry.Positions = new Point3DCollection();
            this.theGeometry.TextureCoordinates = new PointCollection();

            foreach (var vert in vertices)
            {
                this.theGeometry.Positions.Add(new Point3D(vert.X, vert.Y, -vert.Z));
                this.theGeometry.TextureCoordinates.Add(new Point());
            }
        }

        /// <summary>
        /// Sends the new deformed mesh to be drawn
        /// </summary>
        private void UpdateMesh()
        {
            var vertices = this.currentFaceModel.CalculateVerticesForAlignment(this.currentFaceAlignment);

            for (int i = 0; i < vertices.Count; i++)
            {
                var vert = vertices[i];
                this.theGeometry.Positions[i] = new Point3D(vert.X, vert.Y, -vert.Z);
            }
        }

        /// <summary>
        /// Initializes the writing of a file that contains the specified capture points from the face mesh
        /// </summary>
        private void capturePoints()
        {
            // Displays a SaveFileDialog so the user can save the motion capture data
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Capture Friendly Format|*.txt";
            saveFile.Title = "Save Motion File";
            saveFile.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFile.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                recording = !recording;
                frameCount = 0;
                captureButton.Content = "Stop recording";
                recordPoints = new List<HighDetailFacePoints>();
                FacePointCheckList.IsEnabled = false;
                rateSlider.IsEnabled = false;
                string writeFacePoints = null;

                foreach (CheckBox FacePoint in FacePointCheckList.Items)
                {
                    if (FacePoint.IsChecked.Value)
                    {
                        recordPoints.Add((HighDetailFacePoints)Enum.Parse(typeof(HighDetailFacePoints), FacePoint.Content.ToString()));
                        writeFacePoints += FacePoint.Content.ToString() + ", ";
                    }
                }
                fileWriter = new StreamWriter(saveFile.FileName);
                fileWriter.WriteLine(writeFacePoints);
            }
        }

        /// <summary>
        /// Closes the file steam that is recording points captured from the face
        /// </summary>
        private void StopRecording()
        {
             recording = !recording;
                captureButton.Content = "Start recording";
                FacePointCheckList.IsEnabled = true;
                rateSlider.IsEnabled = true;
                if(fileWriter!=null)
                    {
                        fileWriter.Close();
                    }
        }

        /// <summary>
        /// Initiates the process of recording points every frame or stops the process if currently running
        /// </summary>
        private void Capture_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!recording)
            {
                capturePoints();
            }
            else
            {
                StopRecording();
            }
               
        }

        /// <summary>
        /// This event fires when a BodyFrame is ready for consumption
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void BodyReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var frameReference = e.FrameReference;
            using (var frame = frameReference.AcquireFrame())
            {
                if (frame == null)
                {
                    return;
                }

                if (this.currentTrackedBody != null)
                {
                    this.currentTrackedBody = FindBodyWithTrackingId(frame, this.CurrentTrackingId);

                    if (this.currentTrackedBody != null)
                    {
                        return;
                    }
                }

                Body selectedBody = FindClosestBody(frame);  

                if (selectedBody == null)
                {
                    return;
                }
                head = selectedBody.Joints[JointType.Head];

                this.currentTrackedBody = selectedBody;
                this.CurrentTrackingId = selectedBody.TrackingId;

                this.highDefinitionFaceFrameSource.TrackingId = this.CurrentTrackingId;
            }
        }
        
        /// <summary>
        /// This event is fired when a new HDFace frame is ready for consumption
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        /// 

        private void HdFaceReader_FrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            using (var frame = e.FrameReference.AcquireFrame())
            {
                if (frame == null || !frame.IsFaceTracked)
                {
                    return;
                }

                if (recording && head != null)
                {
                    frameCount++;

                    txt_FPS.Text = frameCount.ToString();

                    GeometryConverter convert = new GeometryConverter();


                    if (frameCount % (31 - rateSlider.Value) == 0)
                    {
                        var vertices = this.currentFaceModel.CalculateVerticesForAlignment(this.currentFaceAlignment);
                        string coordinate = null;
         
                        //Steve's code*****//
                        //float minX = 100;
                        //float maxX = 0;
                        //float minY = 100;
                        //float maxY = 0;
                        //Rectangle boundingBox = new Rectangle();
                        //testCanvas.Children.Add(boundingBox);
                        //*****************//
                        foreach (HighDetailFacePoints HDFP in recordPoints)
                        {
                            coordinate = null;
                            CameraSpacePoint spacePoint = vertices[(int)HDFP];

                            float x = spacePoint.X - head.Position.X;
                            float y = spacePoint.Y - head.Position.Y;
                            float z = spacePoint.Z - head.Position.Z;

                            //***************************//
                            //minX = Math.Min(minX, x);
                            //maxX = Math.Max(maxX, x);
                            //minY = Math.Min(minY, y);
                            //maxY = Math.Max(maxY, y);
                            //***************************//

                            coordinate += x + " " + y + " " + z;
                            fileWriter.WriteLine(coordinate);
                        }
                        //********************
                        //boundingBox2.Width = 75;
                        //boundingBox2.Height = 100;
                        //Canvas.SetLeft(boundingBox2, 490+(650*minX));
                        //Canvas.SetTop(boundingBox2, 375+(-600*maxY));

                        //**********************************
                        fileWriter.WriteLine("#");
                    }
                }

                var triangleIndices = this.currentFaceModel.TriangleIndices;

                frame.GetAndRefreshFaceAlignmentResult(this.currentFaceAlignment);
                this.UpdateMesh();
            }
        }
    }
}