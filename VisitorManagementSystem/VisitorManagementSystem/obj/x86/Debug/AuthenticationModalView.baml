   M S B A M L   `   `   `  ����  VisitorManagementSystem5    /VisitorManagementSystem.AuthenticationModalView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      � -   5        $��BaseGrid��5      $	��768��6   $
��1024��6(   ���	#00000000.+�5	   
   � -   5        $��BackgroundGrid��5	      
���   �  J� Opacity$	  0.3Q�68   � 5   
   -   5        $��ContentGrid��5      
�������$��Center=�65   $��Center=�6R   $	��600��6m   $	��258��6y   $��0,0,0,0q�6�    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5         
BlurRadius$ 5Q�6#      	Direction$ 90Q�62      ShadowDepth$ 0Q�6A   .+�5      �� $��Authorisation Required��6   $��22(�65   $��SemiBold"�6C   $��	15,10,0,0q�6Y   $��Center=�6l   $
��Left=�6�   $	��Top=�6�   $��40��6�   $	��575��6�   �� 5      $N��HLogin with your Administrator account to change settings on this system.��6   
���iii�$��16(�6|   $��SemiBold"�6�   $��	15,44,0,0q�6�   $	��Top=�6�   $
��Left=�6�   $	��Top=�6�   $��35��6  $	��575��6  �� 5      -   5        $��UsernameField��5      
���   �$��2q�6X   $��18(�6l   $��40��6z   $��Stretch=�6�   $��21,121,311,0q�6�   $��	10,0,10,0q�6�   $�� ��6�    �� TextWrapping$
 Wrap=�6�   $	��Top=�6�   $��Center=�6   �� SelectionBrush �	#FF0080FF6$  G� 5      -   5         G�Name$ PasswordField��5      
���   �$��2q�6^   $��18(�6r   $	��268��6�   $��40��6�   $��Right=�6�   $��
0,121,21,0q�6�   $��	10,0,10,0q�6�   $	��Top=�6�   $��Center=�6�    G� SelectionBrush �	#FF0080FF6  �� 5      $��0q�6   $��	Username:��6    $
��Left=�64   $��	21,96,0,0q�6O   $	��Top=�6b   $��99��6z   $��15(�6�   �� 5      $��0q�6   $��	Password:��6    $
��Left=�64   $��
311,96,0,0q�6O   $	��Top=�6c   $��99��6{   $��15(�6�   �� 5      -   5        	 ��Name$	 LoginButton��5   �   ���	#FF0080FF
���   �$��0q�6@   $��Login��6T   $��18(�6d   $��SemiBold"�6r   
�������$��40��6�   $��Right=�6�   $��Bottom=�6�   $	��130��6�   $��	0,0,21,21q�6�   6  �� 5      -   5        $	 CancelButton��5   �   ���	#FF909090
���   �$��0q�6@   $��Cancel��6T   $��18(�6e   $��SemiBold"�6s   
�������$��40��6�   $��Right=�6�   $��Bottom=�6�   $	��130��6�   $��
0,0,171,21q�6  6  5      
 � Triggers
 5   
   :� 5       :� RoutedEvent$ Grid.Loaded��6   .[�5      �� .��5       �� .��5!      W�  W� From$	 0.0X�6*   	 W� To$	 0.3X�65    g� Duration$ 0:0:0.2D�6>    �� 
TargetName$ BackgroundGrid��6Q    �� TargetProperty$ Opacity�6x   6�   W� 5"      $	 0.0X�6*   $	 1.0X�65   $ 0:0:0.3D�6>   $ ContentGrid��6Q   $ Opacity�6u   6�   5#      5$      5%       � 	Resources 5(   
      blurAnimation	 t� To  focusAnimation  closeAnimation��%�  &	        &	 |     &	 �     �� 5)      .��5*      t� $ 0,0,0,0X�6%   $ 0:0:0.2D�62   $ ContentGrid��6E   $ Margin�6i   6�   5+      �� 5,      .��5-      t� $ 0:0:0.2D�6%   $ ContentGrid��68   $ Margin�6\   6�   5.      �� 5/      .��50      W� $	 0.0X�6"   $ 0:0:0.4D�6+   $ BackgroundGrid��6>   $ Opacity�6e   6�   W� 51      $	 0.0X�6"   $ 0:0:0.2D�6+   $ ContentGrid��6>   $ Opacity�6b   6�   52      53      54      55      