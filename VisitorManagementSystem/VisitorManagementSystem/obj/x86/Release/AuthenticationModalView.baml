   M S B A M L   `   `   `  ����   VisitorManagementSystem5    /VisitorManagementSystem.AuthenticationModalView   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .��� -   $��BaseGrid��$	��768��$
��1024�����	#00000000.+�� -   $��BackgroundGrid��
���   �  J� Opacity$	  0.3Q�� -   $��ContentGrid��
�������$��Center=�$��Center=�$	��600��$	��258��$��0,0,0,0q� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     
BlurRadius$ 5Q�   	Direction$ 90Q�   ShadowDepth$ 0Q�.+��� $��Authorisation Required��$��22(�$��SemiBold"�$��	15,10,0,0q�$��Center=�$
��Left=�$	��Top=�$��40��$	��575���� $N��HLogin with your Administrator account to change settings on this system.��
���iii�$��16(�$��SemiBold"�$��	15,44,0,0q�$	��Top=�$
��Left=�$	��Top=�$��35��$	��575���� -   $��UsernameField��
���   �$��2q�$��18(�$��40��$��Stretch=�$��21,121,311,0q�$��	10,0,10,0q�$�� �� �� TextWrapping$
 Wrap=�$	��Top=�$��Center=� �� SelectionBrush �	#FF0080FFG� -    G�Name$ PasswordField��
���   �$��2q�$��18(�$	��268��$��40��$��Right=�$��
0,121,21,0q�$��	10,0,10,0q�$	��Top=�$��Center=� G� SelectionBrush �	#FF0080FF�� $��0q�$��	Username:��$
��Left=�$��	21,96,0,0q�$	��Top=�$��99��$��15(��� $��0q�$��	Password:��$
��Left=�$��
311,96,0,0q�$	��Top=�$��99��$��15(��� -   	 ��Name$	 LoginButton�����	#FF0080FF
���   �$��0q�$��Login��$��18(�$��SemiBold"�
�������$��40��$��Right=�$��Bottom=�$	��130��$��	0,0,21,21q��� -   $	 CancelButton�����	#FF909090
���   �$��0q�$��Cancel��$��18(�$��SemiBold"�
�������$��40��$��Right=�$��Bottom=�$	��130��$��
0,0,171,21q�
 � Triggers
 :�  :� RoutedEvent$ Grid.Loaded��.[��� .���� .��W�  W� From$	 0.0X�	 W� To$	 0.3X� g� Duration$ 0:0:0.2D� �� 
TargetName$ BackgroundGrid�� �� TargetProperty$ Opacity�W� $	 0.0X�$	 1.0X�$ 0:0:0.3D�$ ContentGrid��$ Opacity� � 	Resources    blurAnimation	 t� To  focusAnimation  closeAnimation��%%  &	        &	 H     &	 �     �� .��t� $ 0,0,0,0X�$ 0:0:0.2D�$ ContentGrid��$ Margin��� .��t� $ 0:0:0.2D�$ ContentGrid��$ Margin��� .��W� $	 0.0X�$ 0:0:0.4D�$ BackgroundGrid��$ Opacity�W� $	 0.0X�$ 0:0:0.2D�$ ContentGrid��$ Opacity�