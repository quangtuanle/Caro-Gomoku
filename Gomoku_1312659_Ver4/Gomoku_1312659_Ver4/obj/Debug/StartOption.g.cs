﻿#pragma checksum "..\..\StartOption.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1F040F6019C9C6A386FBFCC0ECA424B4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Gomoku_1312659_Ver4;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Gomoku_1312659_Ver4 {
    
    
    /// <summary>
    /// StartOption
    /// </summary>
    public partial class StartOption : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\StartOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnStart;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\StartOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox groupBox;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\StartOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rdoHvH;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\StartOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rdoHvC;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\StartOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rdoSvH;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\StartOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.RadioButton rdoSvC;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\StartOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtName;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\StartOption.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label label;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Gomoku_1312659_Ver4;component/startoption.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\StartOption.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btnStart = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\StartOption.xaml"
            this.btnStart.Click += new System.Windows.RoutedEventHandler(this.btnStart_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.groupBox = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 3:
            this.rdoHvH = ((System.Windows.Controls.RadioButton)(target));
            
            #line 13 "..\..\StartOption.xaml"
            this.rdoHvH.Checked += new System.Windows.RoutedEventHandler(this.rdoHvH_Checked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.rdoHvC = ((System.Windows.Controls.RadioButton)(target));
            
            #line 14 "..\..\StartOption.xaml"
            this.rdoHvC.Checked += new System.Windows.RoutedEventHandler(this.rdoHvC_Checked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.rdoSvH = ((System.Windows.Controls.RadioButton)(target));
            
            #line 15 "..\..\StartOption.xaml"
            this.rdoSvH.Checked += new System.Windows.RoutedEventHandler(this.rdoSvH_Checked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.rdoSvC = ((System.Windows.Controls.RadioButton)(target));
            
            #line 16 "..\..\StartOption.xaml"
            this.rdoSvC.Checked += new System.Windows.RoutedEventHandler(this.rdoSvC_Checked);
            
            #line default
            #line hidden
            return;
            case 7:
            this.txtName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.label = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

