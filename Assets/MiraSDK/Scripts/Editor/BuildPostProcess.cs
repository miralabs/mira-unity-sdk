﻿// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Downloading and/or using this MIRA SDK is under license from MIRA,
// and subject to all terms and conditions of the Mira SDK License Agreement,
// found here: https://www.mirareality.com/Mira_SDK_License_Agreement.pdf
//
// By downloading this SDK, you agree to the Mira SDK License Agreement.
//
// This SDK may only be used in connection with the development of
// applications that are exclusively created for, and exclusively available
// for use with, MIRA hardware devices. This SDK may only be commercialized
// in the U.S. and Canada, subject to the terms of the License.

#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;

public static class BuildPostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // Read Xcode project contents.
            string projectPath = PBXProject.GetPBXProjectPath(path);
            string projectContents = File.ReadAllText(projectPath);
            
            // Parse Xcode project.
            PBXProject project = new PBXProject();
            project.ReadFromString(projectContents);

            // Retrieve target identifier.
            string targetName = PBXProject.GetUnityTargetName();
            string targetGuid = project.TargetGuidByName(targetName);
            string target = project.TargetGuidByName("Unity-iPhone");
            
            project.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            // Retrieve MiraRemote.framework identifier.
            string frameworkGuid = project.FindFileGuidByProjectPath("Frameworks/Plugins/iOS/MiraRemote.framework");
            string wikiFrameworkGuid = project.FindFileGuidByProjectPath("Frameworks/Plugins/iOS/Wikitude/WikitudeMiraSDK.framework");
            // Embed framework in app's bundle.
            PBXProjectExtensions.AddFileToEmbedFrameworks(project, targetGuid, frameworkGuid);
            PBXProjectExtensions.AddFileToEmbedFrameworks(project, targetGuid, wikiFrameworkGuid);

            // Update search paths to include embedded frameworks directory.
            foreach (string name in project.BuildConfigNames()) 
            {
                string guid = project.BuildConfigByName(targetGuid, name);
                project.SetBuildPropertyForConfig(guid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
            }

            project.WriteToFile(projectPath);
        }
    }
}

#endif