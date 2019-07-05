using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace Auroraland
{
    public class VoiceXmlEditor : MonoBehaviour
    {
        public static event VoiceXmlHandler OnCreateVoiceXml;
        public delegate void VoiceXmlHandler(string filename);

        [Header("Grammar Module Files")]
        public string BasicFileName = "BasicGrammarModule.xml";
        public string AssetEditorFileName = "AssetEditorGrammarModule.xml";
        public string EditorFileName = "EditorGrammarModule.xml";
        public string TheaterFileName = "TheaterGrammarModule.xml";
        public string BlackjackFileName = "BlackJackGrammarModule.xml";
        public string TherapyFileName = "TherapyGrammarModule.xml";

        [Header("Includes")]
        public bool HasAssetEditor;
        public bool HasEditor;
        public bool HasTheater;
        public bool HasTherapy;
        public bool HasBlackjack;
        public bool HasUserDefinedRules;

        [Header("Output")]
        public string OutputFileName;
        public string UserDefinedRuleName;
        public List<string> UserDefinedKeywords = new List<string>();

        private XDocument xmlDoc; //the xml doc that used as base template
        private XElement grammarRoot; //the root that specified srgs grammar
        private XElement commandRoot; //the real root
        private Dictionary<string, XElement> ruleDict = new Dictionary<string, XElement>();
        private Dictionary<string, XElement> rulerefDict = new Dictionary<string, XElement>();
        private const string srgsNamespace = "{http://www.w3.org/2001/06/grammar}";
        private const string moduleFolderPath = "/SRGS/Module/";
        private const string srgsFolderPath = "/SRGS/";

        public void CreateXML(bool hasAssetEditor = false, bool hasTheater = false, bool hasBlackjack = false, bool hasTherapy = false, bool hasUserDefined = false)
        {
            HasAssetEditor = hasAssetEditor;
            HasTheater = hasTheater;
            HasTherapy = hasTherapy;
            HasBlackjack = hasBlackjack;
            HasUserDefinedRules = hasUserDefined;
            LoadXML(BasicFileName);
            AppendGrammarModules();
            if (HasUserDefinedRules)
            {
                AppendUserDefinedRules(UserDefinedRuleName, UserDefinedKeywords.ToArray());
            }

            WriteXML();
            if (OnCreateVoiceXml != null)
            {
                OnCreateVoiceXml(srgsFolderPath + OutputFileName);
            }
        }

        public void LoadXML(string fileName) /*Load base template and get grammar and command root*/
        {
            //Assigning Xdocument xmlDoc. Loads the xml file from the file path listed. 
            string path = Application.streamingAssetsPath + moduleFolderPath + fileName;
            xmlDoc = XDocument.Load(path);
            ruleDict.Clear();
            rulerefDict.Clear();

            grammarRoot = xmlDoc.Document.Root;
            List<XElement> rules = grammarRoot.Elements().ToList();
            commandRoot = rules.Where(x => (string)x.Attribute("id") == grammarRoot.Attribute("root").Value).First();
            foreach (XElement rule in rules)
            {
                ruleDict.Add((string)rule.Attribute("id"), rule);
            }
            XElement oneof = commandRoot.Element(srgsNamespace + "one-of");
            foreach (XElement item in oneof.Elements())
            {
                XElement ruleref = item.Element(srgsNamespace + "ruleref");
                //Debug.Log("load "+ruleref.Attribute("uri").Value);
                string key = ruleref.Attribute("uri").Value;
                rulerefDict.Add(key, item);
            }

        }
        public void WriteXML()
        {
            string path = Application.streamingAssetsPath + srgsFolderPath + OutputFileName;
            using (var stream = System.IO.File.Create(path))
            {

                using (var writer = XmlWriter.Create(stream))
                {
                    xmlDoc.Save(writer);
                }
            }
        }

        public void AppendGrammarModules()
        {
            List<XElement> rules = new List<XElement>();
            List<XElement> rulerefs = new List<XElement>();
            if (HasAssetEditor)
            {
                rules.AddRange(GetRulesFromModule(Application.streamingAssetsPath + moduleFolderPath + AssetEditorFileName));
                rulerefs.AddRange(GetRulerefsFromModule(Application.streamingAssetsPath + moduleFolderPath + AssetEditorFileName));
            }
            if (HasEditor)
            {
                rules.AddRange(GetRulesFromModule(Application.streamingAssetsPath + moduleFolderPath + EditorFileName));
                rulerefs.AddRange(GetRulerefsFromModule(Application.streamingAssetsPath + moduleFolderPath + EditorFileName));
            }
            if (HasTheater)
            {
                rules.AddRange(GetRulesFromModule(Application.streamingAssetsPath + moduleFolderPath + TheaterFileName));
                rulerefs.AddRange(GetRulerefsFromModule(Application.streamingAssetsPath + moduleFolderPath + TheaterFileName));
            }
            if (HasTherapy)
            {
                rules.AddRange(GetRulesFromModule(Application.streamingAssetsPath + moduleFolderPath + TherapyFileName));
                rulerefs.AddRange(GetRulerefsFromModule(Application.streamingAssetsPath + moduleFolderPath + TherapyFileName));
            }
            if (HasBlackjack)
            {
                rules.AddRange(GetRulesFromModule(Application.streamingAssetsPath + moduleFolderPath + BlackjackFileName));
                rulerefs.AddRange(GetRulerefsFromModule(Application.streamingAssetsPath + moduleFolderPath + BlackjackFileName));
            }
            foreach (XElement rule in rules)
            {
                if (!ruleDict.ContainsKey(rule.Attribute("id").Value))
                {
                    AppendRule(rule);
                }
                else
                {
                    InsertRule(ruleDict[rule.Attribute("id").Value], rule);
                }
            }
            foreach (XElement item in rulerefs)
            {
                XElement ruleref = item.Element(srgsNamespace + "ruleref");
                if (!rulerefDict.ContainsKey(ruleref.Attribute("uri").Value))
                {
                    AppendRuleRefToRoot(item);
                }
            }
        }

        public void AppendUserDefinedRules(string ruleName, string[] commands)
        {
            //create ruleref to command rule (root)
            XElement ruleref = new XElement(srgsNamespace + "ruleref", new XAttribute("uri", "#" + ruleName));
            XElement child = new XElement(srgsNamespace + "item", ruleref);
            child.SetAttributeValue("repeat", "0-1");
            XElement authtag = new XElement(srgsNamespace + "tag", "out.authority=\"user\";");
            child.Add(authtag);
            commandRoot.Element(srgsNamespace + "one-of").Add(child);

            //create rule
            XElement rule = new XElement(srgsNamespace + "rule", new XAttribute("id", ruleName));
            XElement oneof = new XElement(srgsNamespace + "one-of");
            rule.Add(oneof);
            //create items for voice commands
            foreach (string command in commands)
            {
                XElement item = new XElement(srgsNamespace + "item", command);
                string semanticMeaning = string.Format("out.action = \"{0}\"", command);
                XElement tag = new XElement(srgsNamespace + "tag", semanticMeaning);
                item.Add(tag);
                oneof.Add(item);
            }
            grammarRoot.Add(rule);
        }

        private List<XElement> GetRulesFromModule(string fileName)
        {
            XDocument doc = XDocument.Load(fileName);
            XElement root = doc.Document.Root;
            List<XElement> rules = root.Elements().ToList().Where(x => (string)x.Attribute("id") != root.Attribute("root").Value).ToList();
            return rules;
        }

        private List<XElement> GetRulerefsFromModule(string fileName)
        {
            XDocument doc = XDocument.Load(fileName);
            XElement root = doc.Document.Root;
            XElement commands = root.Elements().ToList().Where(x => (string)x.Attribute("id") == root.Attribute("root").Value).First();
            List<XElement> rulerefs = new List<XElement>();
            foreach (XElement ruleref in commands.Descendants(srgsNamespace + "ruleref").ToList())
            {
                rulerefs.Add(ruleref.Parent);// add item
            }
            return rulerefs;
        }

        private void AppendRuleRefToRoot(XElement ruleref)
        {
            ruleref.Name = srgsNamespace + ruleref.Name.LocalName;
            commandRoot.Element(srgsNamespace + "one-of").Add(ruleref);
        }

        private void InsertRule(XElement target, XElement rule)
        {
            XElement targetOneof = target.Element(srgsNamespace + "one-of");
            XElement oneof = rule.Element(srgsNamespace + "one-of");
            foreach (XElement item in oneof.Elements())
            {
                if (!targetOneof.Elements().Contains(item))
                {
                    item.Name = srgsNamespace + item.Name.LocalName;
                    targetOneof.Add(item);
                }
            }
        }

        private void AppendRule(XElement rule)
        {
            //append the rule to the end of rules
            rule.Name = srgsNamespace + rule.Name.LocalName;
            grammarRoot.Add(rule);
        }
    }
}
