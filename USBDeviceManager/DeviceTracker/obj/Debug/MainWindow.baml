   M S B A M L   `   `   `  ����H  DDeviceTracker, Version=0.0.0.1, Culture=neutral, PublicKeyToken=null-clr-namespace:DeviceTrackerDeviceTracker  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	   A%clr-namespace:DeviceTracker.ViewModelDeviceTracker.ViewModel  5	   	       DeviceTracker.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   0ignorehttp://www.galasoft.ch/ignore     5   	   -localclr-namespace:DeviceTracker     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9system&clr-namespace:System;assembly=mscorlib     5   	   4vm%clr-namespace:DeviceTracker.ViewModel     5	   	     � Title$  Device Tracker��5
   	   $	��446��5   	   $	��537��5   	    � Icon$ Assets/Images/appIcon.ico��5   	    � WindowStartupLocation$ CenterScreen=�5   	    � DataContext 5   	   �� *Main+   Locator �� Source# [   � 	Resources 5       �� MergedDictionaries �� Source # BooleanToVisibilityConverterKey*   $DeviceTracker.EnumToBooleanConverter  EnumToBooleanConverterKey �� 
TargetType	 �� Property
 �� Value  PlaneButton �� 
TargetType ��Name �Name � ShowGridLines ��Name �� Triggers P� Property P� Value �� 
TargetName�� 5   
    5      �� 5      $ Skins/MainSkin.xaml?�6%   %�  &	       &	      '�� &     '�� n     &	 �     �� 5      6+     5      6+   �� 5      " ��6   .�5      �� 	 � ��$
 14(�6-   5      �� 5       " ��6   .�5!      �� 	 � ��$
 14(�6-   5"      �� 5%      " ��6(   .�5'      �� 	 � ��
 �#FFF6/   �� 5(      	 � ��#
 � ��6/   �� 5)      	 � ��$
 0q�64   �� 5*      	 � ��$

 Left=�6?   �� 5+      	 � ��$
 Center=�6=   �� 5,      	 � ��$

 Auto��6*   �� 5-      	 � ��$

 Auto��6+   �� 5.      	 � ��
 5/      �� 50      " ��6*   .R�52      �� $ border��6%   $��0q�65   ._�53   "   � $ btnGrid��6'    .  .+�54   &   �� 
������ ._�55   *   �� $ contentPresenter��6;   #��z��6U   56   '   57   #   58       5:      P� 5<   "    � {� .  . �5=   &   �� $ border��6-   	 �@J�Opacity$	
 0.5Q�6T   �� 5>   &   $ border��6-   	 �@J�Opacity$
 1Q�6T   �� 5?   &   $ contentPresenter��6-   	 �@J�Opacity$	
 0.5Q�6^   5@   #   P� 5B   "    � �� . 5D   #   P� 5F   "    �@J�IsMouseOver . . �5G   &   �� 	 �@�Cursor$

 Hand��6?   5H   #   5J      5L      5M      5O      .��5R      � -   5        $ 
LayoutRoot��5R      .+�5T   
   � $��40,40,40,20q�6    � RowDefinitions 5U      �� 5V      $
��Auto �6    �� 5W      .+�5Z      � -   5        $ deviceSettingGrid��5Z      $��0��6.    5[      �� 5\      $
��Auto �6$   �� 5]      $
��Auto �6$   �� 5^      $
��Auto �6$   �� 5_      $
��Auto �6$   .+�5b      �� -   5         ��Name$ toggleDeviceBackupOption��5b      $��0��5c      $	��Top=�5d      $1��+Enable backup sources from External Devices��5e       d� 	IsChecked 5f      �� *IsDeviceBackupEnabled+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� 5j      -   5        $( "toggleDefaultMediaBackupPathOption��5j      $��1��5k      $��0,15,0,0q�5l      $	��Top=�5m      $5��/Also copy photos/videos from External Media to:��5n       5o      ��  �� Path$% IsDefaultMediaBackupPathEnabled�{�5p      �� $ IsDeviceBackupEnabled�$ TwoWay=�$ PropertyChanged=�]� 5t      $��2��6   $��0,15,0,0q�5u      $	��Top=�5v      {�5w      �� $ IsDeviceBackupEnabled�$ TwoWay=�$ PropertyChanged=�.+�5z      �� $	��240��6   $��30��5{      $��Center=�5|      {�5}      �� $% IsDefaultMediaBackupPathEnabled���5~      �� $ DefaultMediaBackupPath��� 5      -   5         ��Name$ 	browseBtn��5      $��98��5�      $��35��5�      $��7,0,0,0q�5�      ���#00D680$��0q�5�      $��Browse��5�      $��15(�5�      $
��Bold"�5�      
���������5�      �� $" BrowseMediaBackupPathCommand�5�      5�      �� 5�      -   5         ��Name$ 	separator��5�      $��3��5�      $��1��5�      $��0,30,0,0q�5�      $��Stretch=�5�      
�������5�      5�      � 5�      -   5        $ deviceListGrid��5�      $��1��5�      $��0,30,0,0q�5�      $��Stretch=�5�       5�      �� 5�      $
��Auto �6$   �� 5�      .+�5�      ��  �� Orientation$ 
Horizontal=�6   .+�5�      �� -   5         ��Name$ deviceCaption��5�       $��0��5�       $��20(�5�       $��DemiBold"�5�       $��Devices��5�       �� 5�      -	   5        $ refresh��5�      #��[ 5�       � ToolTip$ Refresh��5�      ��5�      �� $ RefreshCommand�.��5�      F� $��18��6   $��18��5�      $��12,0,0,0q�5�      ������    S�3�����8�� H��[ʞ�@ H��� � H�dET�3��s���s��h	� �s���� f��8R� Ւьj� �_Z��M� �i �� �Pt��Pt�hh�Pt�1�j�/�1��� S�1��l%��!@hhI�/��@�I�/��@ST3� I�/��@tˤ �u��63@`�� � �T3� �l� ���� `f@Vz`�    __� �<4����@� ��� `f@+6A/73@ � ����8���	���#00d57f J� RenderTransformOrigin$ 0.5,0.5&�5�        �� Stretch$  Uniform=�5�      z�5�      V� 5�   "   ~�5�   *   �� ! �� Angle$! 0Q�6:   �� 5�   *   " �� ScaleX$" 1Q�69   # �� ScaleY$# 1Q�6D   5�   #   5�      5�      5�      � 5�      -
   5        $ listGrid��5�      $��1��5�      $��0,15,0,0q�5�      .+�5�      �� -   5        $ emptyRectView��5�       $��0q�5�       ���#00d57f$ �� StrokeDashArray$	$ 4 4R�5�       $��2��5�       �� 5�      $��Center=�6    $��Center=�5�       $��Segoe UI Light)�5�       ��5�       �� $ InfoText��� 5�      $��0q�6   $��Stretch=�5�      $��Stretch=�5�      $	��Top=�5�      $��0q�5�      $��Bottom=�5�      $��Disabled=�5�      % � FocusVisualStyle% 5�      Z� ��5�      �� $ StoredDevices�$ PropertyChanged=�$ TwoWay=�y�5�      �� *DefaultView+& �� 	Converter#& [ $ TwoWay=�$ PropertyChanged=���5�      �� 5�      " ��6$   .�5�   "   �� 	 �@��
IsSelected
 6?   �� *
IsSelected+$ TwoWay=�' �� RelativeSource' �� *Self+�� 5�   "   	 � ��
 5�   &   �� 5�   *   " ��6:   .R�5�   .   �� 5�   +   5�   #   5�      ��5�      �� 5�      .R�5�   "   � $��0,7,0,7q�6'   $	��Top=�68   ( � ColumnDefinitions( 5�   &   �� 5�   *   $
��Auto �6;   �� 5�   *   �� 5�   *   $
��Auto �6;   .+�5�   &   �� $ clearImageButton��6-   $��0��5�   -   $��0,0,12,0q�5�   -   $
��Left=�5�   -   $��Center=�5�   -   ) K� ZIndex$) 3��5   -   ��.  #��[ 5  -   $ Clear device��5  -   ��5  -   �� $$ DataContext.ClearDeviceCommand�' �� * �� AncestorType"* ����5  -   �� .��5  *   F� $��20��6/   $��20��5  /   $��0,2,5,0q�5	  /   $
��Left=�5
  /   $��Center=�5  /   �����ȓ����������Hh��ј���Ѡv�h�!ѐ���ֹ�`I�h�!�h�!��ј_�h���h�!����`I���ѐ���}�P��    ���#    ��-@�D
 ��-h��� ��-�=�#��# ��-P�� ��-���C
 ��-�����=�#h�������@�D
��C
   P��	
�������$ 0.5,0.5&�5  /   $  Uniform=�5  /   z�5  .   V� 5  2   .~�5  6   �� $! 0Q�6F   �� 5  6   $" 1Q�6E   $# 1Q�6P   5  3   5  +   5  '   �� 5  &   $��1��60   $
��Left=�5  0   $��14(�5  0   ��5  0   �� *DisplayName+d� 5)  &   + d�Name$+ toggleImportOptionBtn��63   $��2��5*  3   $��40��5+  3   $��20��5,  3   $��Right=�5-  3   
������ 
������ $��0q�50  3   ��51  3   �� $+ %DataContext.ImportOptionToggleCommand�' �� "* ����53  3   ��  54  3   �� *ImportOption+$ TwoWay=�$ PropertyChanged=�#& [ ��58  *   �� 59  .     PlaneToggleButton, �� BasedOn#, [ 64   " d�6a   - �� Triggers- 5:  2   �� 5;  6   . �� Binding. 6B   �� *ImportOption+$ TwoWay=�$ PropertyChanged=�#& [ / �� Value/ True6�   .a�5<  :   �� 	 � ��
 5=  >   �� 5>  B   $&�� /Assets/Images/devices-on-x2.png��6H   5@  ;   5A  7   �� 5C  6   . 6B   �� *ImportOption+$ TwoWay=�$ PropertyChanged=�#& [ 	/ False6�   .a�5D  :   �� 	 � ��
 5E  >   �� 5F  B   $'��!/Assets/Images/devices-off-x2.png��6H   5H  ;   5I  7   5K  /   5O  '   5P  #   5Q     5S     5T     5U     5V     5W     5Y     