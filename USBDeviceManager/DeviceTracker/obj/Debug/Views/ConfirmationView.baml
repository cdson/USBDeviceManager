   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5   	   H DDeviceTracker, Version=0.0.0.1, Culture=neutral, PublicKeyToken=nullA%clr-namespace:DeviceTracker.ViewModelDeviceTracker.ViewModel 5   	   *   $DeviceTracker.Views.ConfirmationView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   0ignorehttp://www.galasoft.ch/ignore     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9system&clr-namespace:System;assembly=mscorlib     5   	   4vm%clr-namespace:DeviceTracker.ViewModel     5   	     � Title$  Device Tracker��5	   	   $	��480��5
   	   $	��140��5   	    � Icon$  /Assets/Images/appIcon.ico��5   	    � 
ResizeMode$ NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5   	    � DataContext 5   	   �� *ConfirmationVm+   Locator �� Source# [   � 	Resources 5       �� 
TargetType �� Property	 �� Value��%v   '��       �� 5   
   " ��6   .�5      ��  � ��$	 75��6&   �� 5       � ��$	 25��6'   5      5      .��5      � 
 � ColumnDefinitions
 5   
   �� 5      $
��Auto �6   �� 5      $��2* �6   .+�5!   
   �� $��0��6   $��
25,20,25,0q�5"      $	��Top=�5#      $ ��/Assets/Images/appIcon.ico��5$      $��UniformToFill=�5%      � 5'   
   $��1��6    � RowDefinitions 5(      �� 5)      $
��Auto �6    �� 5*      $
��Auto �6    �� 5+      $
��Auto �6    
 5.      �� 5/      �� 50      $
��Auto �6#   �� 51      $
��Auto �6#   .+�54      ]� $��0��6   $��3��6%   .+�55      �� $
��Left=�6   $��14(�56      $
��Bold"�57      $��Backup files from ��58      �� 5:      -   5        $��
deviceName��5:      $��Right=�5;      $��14(�5<      $
��Bold"�5=      5>      �� 5A      $��1��6   $��3��5B      $��0,12,0,0q�5C      $-��'Save photos and videos from this device��5D      �� 5G      -   5         ��Name$ btnNever��5G      $��2��5H      $��0��5I      $��	0,12,0,12q�5J      $
��Left=�5K      $	��Top=�5L      $��Never��5N      �� 5Q      -   5        $ 	btnAlways��5Q      $��2��5R      $��1��5S      $��
0,12,10,12q�5T      $	��Top=�5U      $��Always��5W      �� 5Y      -   5        $ btnJustOnce��5Y      $��3��5Z      $��2��5[      $��
0,12,12,12q�5\      $	��Top=�5]      $��	Just once��5_      5`      5b      5c      