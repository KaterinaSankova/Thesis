﻿#pragma checksum "..\..\..\..\..\Views\FolderInputView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A37391E0EC8E376C84968740BB4D45B97B60B249"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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
using TSP.ViewModels;
using TSP.Views;


namespace TSP.Views {
    
    
    /// <summary>
    /// FolderInputView
    /// </summary>
    public partial class FolderInputView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 36 "..\..\..\..\..\Views\FolderInputView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock SourceFolderTextBlock;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\..\Views\FolderInputView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ResultFolderTextBlock;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\..\..\Views\FolderInputView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock OutputFolderTextBlock;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.2.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TSP;component/views/folderinputview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\FolderInputView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.2.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SourceFolderTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            
            #line 37 "..\..\..\..\..\Views\FolderInputView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SearchForSourceFolder);
            
            #line default
            #line hidden
            return;
            case 3:
            this.ResultFolderTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            
            #line 43 "..\..\..\..\..\Views\FolderInputView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SearchForResultFolder);
            
            #line default
            #line hidden
            return;
            case 5:
            this.OutputFolderTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            
            #line 55 "..\..\..\..\..\Views\FolderInputView.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SearchForOutputFolder);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

