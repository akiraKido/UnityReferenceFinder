using FluentAssertions;
using UnityReferenceFinder.YamlParser.Nodes;
using Xunit;
// ReSharper disable PossibleNullReferenceException

namespace UnityReferenceFinder.YamlParser.Tests {
    public class ParserTests
    {
        private YamlObject TestGeneral(string text, int childCount)
        {
            var result = new UnityYamlTree().Parse(text);
            result.YamlNodeType.Should().Be(YamlNodeType.UnityYamlFile);

            var fileNode = (UnityYamlFile) result;
            fileNode.FileId.Should().Be("1");
            fileNode.UnityId.Should().Be("29");

            fileNode.Block.Count.Should().Be(1);
            
            var block = fileNode.Block["OcclusionCullingSettings"];
            block.YamlNodeType.Should().Be(YamlNodeType.Object);
            var obj = (YamlObject) block;
            obj.Values.Count.Should().Be(childCount);

            (obj.Values["m_ObjectHideFlags"] as YamlScalar).Value.Should().Be("0");
            (obj.Values["serializedVersion"] as YamlScalar).Value.Should().Be("2");
            (obj.Values["m_SceneGUID"] as YamlScalar).Value.Should().Be("00000000000000000000000000000000");

            return obj;
        }

        private void TestNestedObjectImpl(YamlObject block)
        {
            var rawNestedObject = block.Values["m_OcclusionBakeSettings"];
            rawNestedObject.YamlNodeType.Should().Be(YamlNodeType.Object);

            var nestedObject = (YamlObject) rawNestedObject;
            nestedObject.Values.Count.Should().Be(3);
            (nestedObject.Values["smallestOccluder"] as YamlScalar).Value.Should().Be("5");
            (nestedObject.Values["smallestHole"] as YamlScalar).Value.Should().Be("0.25");
            (nestedObject.Values["backfaceThreshold"] as YamlScalar).Value.Should().Be("100");
        }

        private void TestLiteralDictionaryImpl(YamlObject block)
        {
            var rawLiteralDictionary = block.Values["m_OcclusionCullingData"];
            rawLiteralDictionary.YamlNodeType.Should().Be(YamlNodeType.Object);

            var literalDictionary = (YamlObject) rawLiteralDictionary;
            literalDictionary.Values.Count.Should().Be(2);
            (literalDictionary.Values["fileID"] as YamlScalar).Value.Should().Be("0");
            (literalDictionary.Values["hoge"] as YamlScalar).Value.Should().Be("2");
        }
        
        [Fact]
        public void FileTest()
        {
            var text = @"
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_SceneGUID: 00000000000000000000000000000000
".Trim();
            TestGeneral(text, 3);
        }

        [Fact]
        public void TestNestedObject()
        {
            var text = @"
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
".Trim();
            var block = TestGeneral(text, 4);
            TestNestedObjectImpl(block);
        }

        [Fact]
        public void TestLiteralDictionary()
        {
            var text = @"
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0, hoge: 2}
".Trim();
            var block = TestGeneral(text, 5);
            TestNestedObjectImpl(block);
            TestLiteralDictionaryImpl(block);
        }

        [Fact]
        public void RealLifeTest()
        {
            var text = @"
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_FogColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {r: 0.212, g: 0.227, b: 0.259, a: 1}
  m_AmbientEquatorColor: {r: 0.114, g: 0.125, b: 0.133, a: 1}
  m_AmbientGroundColor: {r: 0.047, g: 0.043, b: 0.035, a: 1}
  m_AmbientIntensity: 1
  m_AmbientMode: 3
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 0}
  m_IndirectSpecularColor: {r: 0, g: 0, b: 0, a: 1}
  m_UseRadianceAmbientProbe: 0".Trim();
            var result = new UnityYamlTree().Parse(text);
            result.YamlNodeType.Should().Be(YamlNodeType.Object);

            var renderSettings = (YamlObject) result;
            renderSettings.Values.Count.Should().Be(28);

            var fogColorA = renderSettings["m_FogColor"]["a"];
            fogColorA.AsString().Should().Be("1");
            fogColorA.AsInt().Should().Be(1);

            var spotCookieGuid = renderSettings["m_SpotCookie"]["guid"];
            spotCookieGuid.AsString().Should().Be("0000000000000000e000000000000000");
            spotCookieGuid.TryAsInt(out _).Should().BeFalse();
        }

        [Fact]
        public void TestLists()
        {
            var text = @"
GameObject:
  m_Component:
  - component: {fileID: 1495266950}
    propertyPath: m_AnchorMin.x
    value: 0
  - component: {fileID: 1495266949, 
    test: 1}
    propertyPath: m_AnchorMin.y
    value: 0
".Trim();
            var result = new UnityYamlTree().Parse(text);

            var gameObject = (YamlObject) result;
            gameObject.Values.Count.Should().Be(1);
            
            var mComponent = gameObject["m_Component"];
            mComponent.YamlNodeType.Should().Be(YamlNodeType.List);
            mComponent.As<YamlList>().Count().Should().Be(2);
            mComponent.As<YamlList>()[0]["component"]["fileID"].AsString().Should().Be("1495266950");
            mComponent.As<YamlList>()[0]["propertyPath"].AsString().Should().Be("m_AnchorMin.x");
            mComponent.As<YamlList>()[0]["value"].AsInt().Should().Be(0);
            
            mComponent.As<YamlList>()[1]["component"]["fileID"].AsString().Should().Be("1495266949");
            mComponent.As<YamlList>()[1]["component"]["test"].AsInt().Should().Be(1);
            mComponent.As<YamlList>()[1]["propertyPath"].AsString().Should().Be("m_AnchorMin.y");
            mComponent.As<YamlList>()[1]["value"].AsInt().Should().Be(0);
        }
    }
}