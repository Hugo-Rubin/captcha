using System;
using System.IO;

namespace Core.Common
{
    public static class DirectoryManager
    {
        public static DirectoryInfo SamplesDirectory
        {
            get
            {
                return new DirectoryInfo(string.Format(@"{0}\Samples", SolutionDirectory.FullName));
            }
        }

        private static DirectoryInfo solutionDirectory;

        public static DirectoryInfo SolutionDirectory
        {
            get
            {
                if (solutionDirectory == null)
                {
                    var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
                    if (directoryInfo != null)
                    {
                        if (directoryInfo.Parent != null)
                        {
                            solutionDirectory = directoryInfo.Parent;
                            return solutionDirectory;
                        }
                    }
                    throw new Exception("A estrutura de pastas da solution deve ter mudado. Corrija GetSolutionDirectory()");
                }
                return new DirectoryInfo(solutionDirectory.FullName);
            }
        }
    }
}
