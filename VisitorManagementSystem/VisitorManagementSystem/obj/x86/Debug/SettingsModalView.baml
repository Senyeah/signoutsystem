   M S B A M L   `   `   `  ����  VisitorManagementSystem/    )VisitorManagementSystem.SettingsModalView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      � -   5        $��BaseGrid��5      $	��768��6   $
��1024��6(   ���	#00000000.+�5	   
   � -   5        $��BackgroundGrid��5	      
���   �  J� Opacity$	  0.3Q�68   � 5   
   -   5        $��ContentGrid��5      
�������$��Center=�65   $��Center=�6R   $	��600��6m   $	��768��6y    J� Effect 5      3  -System.Windows.Media.Effects.DropShadowEffect  5         
BlurRadius$ 5Q�6#      	Direction$ 90Q�62      ShadowDepth$ 0Q�6A   .+�5      �� $��System Settings��6   $��22(�6.   $��SemiBold"�6<   $��	15,10,0,0q�6R   $��Center=�6e   $
��Left=�6�   $	��Top=�6�   $��40��6�   $	��575��6�   �� 5      -   5         ��Name$ OKButton��5   �   ���	#FF0080FF
���   �$��0q�6@   $��OK��6T   $��18(�6a   $��SemiBold"�6o   
�������$��40��6�   $��Right=�6�   $��Bottom=�6�   $	��130��6�   $��	0,0,21,21q�6�   �� 5      -   5        $ CancelButton��5   �   ���	#FF909090
���   �$��0q�6@   $��Cancel��6T   $��18(�6e   $��SemiBold"�6s   
�������$��40��6�   $��Right=�6�   $��Bottom=�6�   $	��130��6�   $��
0,0,171,21q�6  6  �� 5      -   5        $��SystemNameLabel��5   �   $��VisitorSystem-PC.local��6   $��40��65   $��15(�6A   $��Center=�6O   $��Right=�6q   $��Right=�6�   $��	0,10,10,0q�6�   $	��Top=�6�   $	��162��6�   �� 5      -   5        $��label1��5   �   $��Information��6   $��15(�6*   $��40��68   $��SemiBold"�6D   $
��Left=�6Z   $��	15,50,0,0q�6u   $	��Top=�6�   $��Center=�6�   $	��585��6�   �� 5      -   5        $��label2��5   x   $��Server Connection��6   $��15(�60   $��40��6>   $
��Left=�6J   $��	15,86,0,0q�6e   $	��Top=�6�   $��Center=�6�   $	��141��6�   �� 5      -	   5        $��label3��5   x   $��Quit Application��6   $��15(�6/   $��40��6=   $
��Left=�6I   $��
15,343,0,0q�6d   $	��Top=�6�   $��Center=�6�   $	��585��6�   �� 5      -
   5        $��
QuitButton��5   �   
���  ��
���   �$��0q�6:   $
��Quit��6N   $��15(�6]   
�������$��Right=�6~   $��0,347,10,389q�6�   $	��100��6�   6�   �� 5      -   5         ��Name$! ServerConnectionStatusLabel��5   }   $��Last ping was 10s ago��6   $��15(�64   $��40��6B   $
��Left=�6N   $��
156,86,0,0q�6i   $	��Top=�6�   $��Center=�6�   $��Right=�6�   $	��434��6�   �� 5      -   5        $ label2_Copy��5   z   $��Current Session ID��6   $��15(�61   $��40��6?   $
��Left=�6K   $��
15,126,0,0q�6f   $	��Top=�6�   $��Center=�6�   $	��141��6�   �� 5      -   5        $ SessionIDLabel��5   m   $
��null��6   $��15(�6#   $��40��61   $
��Left=�6=   $��156,126,0,0q�6X   $	��Top=�6�   $��Center=�6�   $��Right=�6�   $	��434��6�   �� 5       -   5        $ label2_Copy1��5    s   $��System Name��6   $��15(�6*   $��40��68   $
��Left=�6D   $��
15,166,0,0q�6_   $	��Top=�6�   $��Center=�6�   $	��141��6�   �� 5!      -   5        $ SessionIDLabel_Copy��5!   r   $��	VIRTUALPC��6   $��15(�6(   $��40��66   $
��Left=�6B   $��156,166,0,0q�6]   $	��Top=�6�   $��Center=�6�   $��Right=�6�   $	��434��6�   5"       � 	Resources 5$   
      blurAnimation	 t� To	 g� Duration
 �� 
TargetName �� TargetProperty  focusAnimation  closeAnimation	 W� To��%�  &	        &	 |     &	 �     �� 5%      .��5&      t� $ 0,0,0,0X�6%   $	 0:0:0.2D�62   $
 ContentGrid��6E   $ Margin�6i   6�   5'      �� 5(      .��5)      t� $	 0:0:0.2D�6%   $
 ContentGrid��68   $ Margin�6\   6�   5*      �� 5+      .��5,      W� $	 0.0X�6"   $	 0:0:0.3D�6+   $
 BackgroundGrid��6>   $ Opacity�6e   6�   W� 5-      $	 0.0X�6"   $	 0:0:0.2D�6+   $
 ContentGrid��6>   $ Opacity�6b   6�   5.      5/      50      51      