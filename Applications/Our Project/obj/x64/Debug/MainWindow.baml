   M S B A M L   `   `   `  ����H  DKinectKapture, Version=0.9.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:TeamBest.KinectKaptureTeamBest.KinectKapture  5   	   '    !TeamBest.KinectKapture.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   6local$clr-namespace:TeamBest.KinectKapture     5   	   -   5          � Title$  Kinect Kapture��5   	   $	��735��5   	   $	��770��6    � 	Resources 5	         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value��%�   &	        &	 #     &	 F     '�� i     �� 5
   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5   
   $ 	#ff00BCF2��62   �� 5   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      5      .��5      � -   5        $��
layoutGrid��5      $��
10 0 10 10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ 
txt_Status��5      $��1��6(   ��66   �� *
StatusText+�� 5   
   $��2��6    �� Orientation$ 
Horizontal=�6"   .+�5      �� -   5        	 ��Name$	 captureButton��5      .��6I   Start Capture6X   �� 5      $��100, 0, 0, 0q�6    Capture Rate (Captures/Second)6N   �� 5      -   5        
 ��Name$
 
rateSlider��5      $	��281��6)    � Maximum$ 30Q�65    � Minimum$ 1Q�6B    �� Interval$ 1��6N    �� TickFrequency$ 1Q�6[    � Value$ 30Q�6m    �� Ticks$V P1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30R�6x    �� TickPlacement$
 Both=�6�    �� IsSnapToTickEnabled . 6  �� 5      $��
0, 0, 0, 0q�6   ��6,   ��  �� ElementName$ 
rateSlider�� �� Path$ Value�6b   �� 5      2  ,System.Windows.Markup.XmlAttributeProperties  XmlSpace preserve6-   $��25, 0, 0, 0q�6   Frame:6J   �� 5       -   5        $ txt_FPS��5       $��
0, 0, 0, 0q�6)   06@   5!      �� 5"   
   5#      � 5$   
   $��3��6   $��2��6    � ColumnDefinitions 5%      �� 5&      $
��auto �6#   �� 5'      $��* �6#   .+�5)      '� -   5         '�Name$ 
viewport3d��5)      $��1��6   $��Stretch=�6*   $��Stretch=�6H   $
��Auto��6d   $
��Auto��6q    J� RenderTransformOrigin$ 0.5,0.5&�6�   $��
0,10,0,-10q�6�   z�5*      �� 5+       �� ScaleX$	 2.0Q�6%    �� ScaleY$	 2.0Q�62    '� Camera 51      =� 52       � Position$ 0.0, 0.0, 0.451�6(    � LookDirection$ 0,0, -15�6B    � UpDirection$ 0,1,05�6[    =� FieldOfView$ 70Q�6o   .x�54      g�   g� Content  55      t� 56      ! �� Color$! #FFFFFF��6+   " t� 	Direction$" -0.5,-0.5,-0.55�6;   58      g� 59        5:      � 5;      # � Geometry# 5?      k� 5@   "   -   5        theGeometry��$ k� TriangleIndices$ �       % k� Normals% �   & k� TextureCoordinates& �   ' k� 	PositionsK' �   p/��p/���� p/���� �� �� �� p/���� p/��p/��5D      ( � Material( 5H      �� 5I   "   .��5J   &   u� -	   5        theMaterial��) u� Brush) 5K   *   �� 5L   .   * �� 
StartPoint$* 0,0.5&�6B   + �� EndPoint$+ 1,0.5&�6U   .��5M   2   � , � Color$, Yellow��6?   - � Offset$- 0Q�6N   � 5N   2   $	, Red��6?   $
- 0.25Q�6K   � 5O   2   $
, Blue��6?   $
- 0.75Q�6L   � 5P   2   $, 	LimeGreen��6?   $- 1Q�6Q   5Q   /   5S   '   5T   #   . j� 	Transform. 5X      �� 5Y   "   / �� Rotation/ 5Z   &   �� 5[   *   0 �� Axis$0 0,3,05�6>   1 �� Angle$1 0Q�6K   5]   #   5_      5a      5c      �� 5i      -
   5        2 ��Name$2 FacePointCheckList��5i      $��0��63   5j      5k      5l      