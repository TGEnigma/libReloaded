﻿/*
    [Reloaded] Mod Loader Common Library (libReloaded)
    The main library acting as common, shared code between the Reloaded Mod 
    Loader Launcher, Mods as well as plugins.
    Copyright (C) 2018  Sewer. Sz (Sewer56)

    [Reloaded] is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    [Reloaded] is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Reloaded.IO
{
    /// <summary>
    /// Evaluates all of the files in the present directory and subdirectories of a specified directory and returns their relative paths to
    /// the requested directory/
    /// </summary>
    public static class RelativePaths
    {
        /// <summary>
        /// Creates a hardlink for an already existing specific file elsewhere at another path.
        /// </summary>
        /// <param name="lpFileName">The name of the new file. This parameter may include the path but cannot specify the name of a directory.</param>
        /// <param name="lpExistingFileName">The name of the existing file. This parameter may include the path cannot specify the name of a directory.</param>
        /// <param name="lpSecurityAttributes">Reserved, should be set to null (IntPtr.Zero).</param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        /// <summary>
        /// Specifies the method by which files are meant to be copied for copy operations.
        /// Files can either be literally copied with their data being copied or a hardlink 
        /// (think something like a pointer to the actual file data on the hard drive, and you're just copying the reference to where the data is stored).
        /// </summary>
        public enum FileCopyMethod
        {
            /// <summary>
            /// Copies the files, literally from A to B.
            /// </summary>
            Copy,
            /// <summary>
            /// Creates a hardlink from target A to destination B, both A & B still point to the same physical data on the hard disk.
            /// </summary>
            Hardlink
        }

        /// <summary>
        /// Retrieves the relative paths of all files for the directory currently set.
        /// </summary>
        /// <param name="directory">
        /// Defines the absolute location of the directory for which all relative file paths are meant to be evaluated for all subfolders and files.
        /// The directory path should not end on a backslash.
        /// </param>
        /// <returns>A list of strings with relative file paths</returns>
        public static List<string> GetRelativeFilePaths(string directory)
        {
            // Return the relative file list.
            return Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories).Select(x => x.Replace(directory, "")).ToList();
        }

        /// <summary>
        /// Copies a list of files by relative path from a list of relative paths to a specified set target directory.
        /// </summary>
        /// <param name="relativePaths">Specifies the relative file paths which are to be copied.</param>
        /// <param name="sourceDirectory">Specifies the source directory from which the files are meant to be copied from. Should not end on a back/forward slash.</param>
        /// <param name="targetDirectory">Specifies the arget directory to which the files are meant to be copied. Should not end on a back/forward slash.</param>
        /// <param name="fileCopyMethod">Specifies the way the files will be copied from A to B.</param>
        public static void CopyByRelativePath(List<string> relativePaths, string sourceDirectory, string targetDirectory, FileCopyMethod fileCopyMethod)
        {
            // For each relative path.
            foreach(string relativePath in relativePaths)
            {
                // Get the target path.
                string targetPath = targetDirectory + relativePath;

                // Get the source path.
                string sourcePath = sourceDirectory + relativePath;

                // Confirm source, and target's directory exist.
                if (! File.Exists(sourcePath)) continue;
                if (! Directory.Exists(Path.GetDirectoryName(targetPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

                // Copy the files from A to B using the specified target method.
                CopyWithMethod(sourcePath, targetPath, fileCopyMethod);
            }
        }

        /// <summary>
        /// Copies a list of files by relative path from a list of relative paths to a specified set target directory.
        /// </summary>
        /// <param name="sourceDirectory">Specifies the source directory from which the files are meant to be copied from. Should not end on a back/forward slash.</param>
        /// <param name="targetDirectory">Specifies the arget directory to which the files are meant to be copied. Should not end on a back/forward slash.</param>
        /// <param name="fileCopyMethod">Specifies the way the files will be copied from A to B.</param>
        public static void CopyByRelativePath(string sourceDirectory, string targetDirectory, FileCopyMethod fileCopyMethod)
        {
            // Obtain the relative paths to the target directory.
            List<string> relativePaths = GetRelativeFilePaths(sourceDirectory);

            // Call the other overload.
            CopyByRelativePath(relativePaths, sourceDirectory, targetDirectory, fileCopyMethod);
        }

        /// <summary>
        /// Copies a file from A to B using the specified target method. Assumes target directory already exists.
        /// </summary>
        /// <param name="fileCopyMethod">The method bu which the file is meant to be copied.</param>
        /// <param name="sourcePath">The path where the file that is to be copied lies.</param>
        /// <param name="targetPath">The path where the file at sourcePath is to be copied to.</param>
        private static void CopyWithMethod(string sourcePath, string targetPath, FileCopyMethod fileCopyMethod)
        {
            switch (fileCopyMethod)
            {
                case FileCopyMethod.Copy: File.Copy(sourcePath, targetPath, true); break;
                case FileCopyMethod.Hardlink:

                    // Try creating hardlink.
                    // If the operation fails, copy the file with replacement.
                    if (CreateHardLink(targetPath, sourcePath, IntPtr.Zero) == false) File.Copy(sourcePath, targetPath, true);

                    break;
            }
        }
    }
}
