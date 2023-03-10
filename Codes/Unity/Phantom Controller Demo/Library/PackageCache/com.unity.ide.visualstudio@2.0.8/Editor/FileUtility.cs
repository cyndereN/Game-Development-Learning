/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Unity Technologies.
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal static class FileUtility
	{
		public const char WinSeparator = '\\';
		public const char UnixSeparator = '/';

		// Safe for packages as we use packageInfo.resolvedPath, so this should work in library package cache as well
		public static string[] FindPackageAssetFullPath(string assetfilter, string filefilter)
		{
			return AssetDatabase.FindAssets(assetfilter)
				.Select(AssetDatabase.GUIDToAssetPath)
				.Where(assetPath => assetPath.Contains(filefilter))
				.Select(asset =>
				 {
					 var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(asset);
					 return Normalize(packageInfo.resolvedPath + asset.Substring(packageInfo.assetPath.Length));
				 })
				.ToArray();
		}

		public static string GetAssetFullPath(string asset)
		{
			var basePath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
			return Path.GetFullPath(Path.Combine(basePath, Normalize(asset)));
		}

		public static string Normalize(string path)
		{
			if (string.IsNullOrEmpty(path))
				return path;

			if (Path.DirectorySeparatorChar == WinSeparator)
				path = path.Replace(UnixSeparator, WinSeparator);
			if (Path.DirectorySeparatorChar == UnixSeparator)
				path = path.Replace(WinSeparator, UnixSeparator);

			return path.Replace(string.Concat(WinSeparator, WinSeparator), WinSeparator.ToString());
		}

		public static string NormalizeWindowsToUnix(string path)
		{
			if (string.IsNullOrEmpty(path))
				return path;

			return path.Replace(WinSeparator, UnixSeparator);
		}

		internal static bool IsFileInProjectRootDirectory(string fileName)
		{
			var relative = MakeRelativeToProjectPath(fileName);
			if (string.IsNullOrEmpty(relative))
				return false;

			return relative == Path.GetFileName(relative);
		}
		
		// returns null if outside of the project scope
		internal static string MakeRelativeToProjectPath(string fileName)
		{
			var basePath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
			fileName = Normalize(fileName);

			if (!Path.IsPathRooted(fileName))
				fileName = Path.Combine(basePath, fileName);

			if (!fileName.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
				return null;

			return fileName
				.Substring(basePath.Length)
				.Trim(Path.DirectorySeparatorChar);
		}

	}
}
