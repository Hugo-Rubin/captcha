using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Common;
using Core.Logic.Filtros;
using Core.Logic.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace IntegrationTests
{
    [TestClass]
    public class GringoFilterTest
    {
        private static readonly Image Bmp = Image.FromFile(string.Format(@"{0}\CaptchaRF\001.png", DirectoryManager.SamplesDirectory));
        private static Dictionary<GringoFilterType, ImgArray> resultList = new Dictionary<GringoFilterType, ImgArray>();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            resultList = RunAllGringoFilters(Bmp);
        }

        [TestMethod]
        public void CanApplyBackgroundMask()
        {
            const GringoFilterType filter = GringoFilterType.BACKGROUND_MASK;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyBitmap()
        {
            const GringoFilterType filter = GringoFilterType.BITMAP;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyCasrSeguiments()
        {
            const GringoFilterType filter = GringoFilterType.CASR_SEGUIMENTS;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyContours()
        {
            const GringoFilterType filter = GringoFilterType.CONTOURS;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyCountorLimit()
        {
            const GringoFilterType filter = GringoFilterType.COUNTOR_LIMIT;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyCountorGraph()
        {
            const GringoFilterType filter = GringoFilterType.COUNTOUR_GRAPH;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyCountourLevelSets()
        {
            const GringoFilterType filter = GringoFilterType.COUNTOUR_LEVEL_SETS;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyCountourPlusGraph()
        {
            const GringoFilterType filter = GringoFilterType.COUNTOUR_LS_PLUS_GRAPH;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyDirectionalConflict()
        {
            const GringoFilterType filter = GringoFilterType.DIRECTIONAL_CONFLICT;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyEnclosedSpace()
        {
            const GringoFilterType filter = GringoFilterType.ENCLOSED_SPACE;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyErosionGradient()
        {
            const GringoFilterType filter = GringoFilterType.EROSION_GRADIENT;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyGraphPlusChainRoots()
        {
            const GringoFilterType filter = GringoFilterType.GRAPH_PLUS_CHAIN_ROOTS;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyRawDcSkeletonEndpoints()
        {
            const GringoFilterType filter = GringoFilterType.RAW_DC_SKELETON_ENDPOINTS;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplySource()
        {
            const GringoFilterType filter = GringoFilterType.SOURCE;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        [TestMethod]
        public void CanApplyZsSkeleton()
        {
            const GringoFilterType filter = GringoFilterType.ZS_SKELETON;
            resultList.Keys.Should().Contain(filter);
            resultList[filter].Should().Not.Be.Null();
        }

        private static Dictionary<GringoFilterType, ImgArray> RunAllGringoFilters(Image image)
        {
            var result = new Dictionary<GringoFilterType, ImgArray>();
            var folder = string.Empty;
            foreach (var filter in Enum.GetValues(typeof(GringoFilterType)))
            {
                var gringoFilter = new GringoFilter();
                var imgArray = gringoFilter.Apply(image, (GringoFilterType)filter);
                folder = SaveResult(imgArray, string.Format("{0}.png", filter));
                result.Add((GringoFilterType)filter, imgArray);
            }
            if (string.IsNullOrEmpty(folder) == false)
            {
                Process.Start(folder); 
            }
            return result;
        }
        
        private static string SaveResult(ImgArray imgArray,string fileName)
        {
            var baseFolder = string.Format(@"{0}\IntegrationTests\Results\GringoFilter\", DirectoryManager.SolutionDirectory);

            var fullName = baseFolder + fileName;
            if (File.Exists(fullName))
            {
                File.Delete(fullName);
            }

            imgArray.Save(fullName);
        }
    }
}
