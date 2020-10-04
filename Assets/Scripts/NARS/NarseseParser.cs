using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarseseParser : MonoBehaviour
{
    public class Statement
    {
        public SubjectPredicate subjectPredicate;
        public TruthValue truthValue;
        public StatementType type;

        public enum StatementType
        {
            Inheritance,
            Similarity,
            Unsupported
        }

        public Statement(string statement)
        {
            int indexOfCopula = statement.IndexOf(NarseseParser.INHERITANCE_COPULA); // -->
            int copulaLength = NarseseParser.INHERITANCE_COPULA.Length;
            type = StatementType.Inheritance;
            if (indexOfCopula == -1) // not found, similarity statement
            {
                indexOfCopula = statement.IndexOf(NarseseParser.SIMILARITY_COPULA); // <->
                copulaLength = NarseseParser.SIMILARITY_COPULA.Length;
                type = StatementType.Similarity;
            }

            if (indexOfCopula == -1) //not found, unsupported
            {
                type = StatementType.Unsupported;
                return;
            } 

            ((string, string), (string, string)) parsedStatement = ParseStatement(statement, indexOfCopula, copulaLength);
            subjectPredicate = new SubjectPredicate(parsedStatement.Item1.Item1, parsedStatement.Item1.Item2);
            truthValue = new TruthValue(parsedStatement.Item2.Item1, parsedStatement.Item2.Item2);
        }

        //returns (subject, predicate), (frequency, confidence)
        protected ((string, string), (string, string)) ParseStatement(string statement, int indexOfCopula, int copulaLength)
        {
            Debug.Log("parsing statement " + statement);

            int indexOfStart = statement.IndexOf("<") + 1;
            int indexOfEnd = statement.LastIndexOf(">.");
            string subject = statement.Substring(indexOfStart, indexOfCopula - indexOfStart).Trim(' ');
            string predicate = statement.Substring(indexOfCopula + copulaLength, indexOfEnd - (indexOfCopula + copulaLength)).Trim(' ');

            int indexOfTruthValue = statement.IndexOf("%") + 1;
            int indexEndOfTruthValue = statement.LastIndexOf("%");
            int indexOfTruthValueSeparator = statement.IndexOf(";", indexOfTruthValue);
            string strFreq = statement.Substring(indexOfTruthValue, indexOfTruthValueSeparator - indexOfTruthValue);
            string strConf = statement.Substring(indexOfTruthValueSeparator + 1, indexEndOfTruthValue - (indexOfTruthValueSeparator + 1));

            string frequency = strFreq;
            string confidence = strConf;

            return ((subject, predicate), (frequency, confidence));
        }

        public override int GetHashCode()
        {
            return subjectPredicate.GetHashCode();
        }
    }

    public class SubjectPredicate
    {
        public string _subject;
        public string _predicate;
        public SubjectPredicate(string subject, string predicate)
        {
            _subject = subject;
            _predicate = predicate;
        }

        public override int GetHashCode()
        {
            return _subject.GetHashCode() + _predicate.GetHashCode();
        }
    }

    public class TruthValue
    {
        public string _frequency;
        public string _confidence;
        public TruthValue(string frequency, string confidence)
        {
            _frequency = frequency;
            _confidence = confidence;
        }

        public override int GetHashCode()
        {
            return _frequency.GetHashCode() + _confidence.GetHashCode();
        }
    }

    //conversion rule
    public static Statement GetReverseStatement(string statement)
    {
        Statement reversedStatement = new Statement(statement);
        reversedStatement.subjectPredicate = new SubjectPredicate(reversedStatement.subjectPredicate._predicate, reversedStatement.subjectPredicate._subject);
        float fTimesC = System.Single.Parse(reversedStatement.truthValue._frequency) * System.Single.Parse(reversedStatement.truthValue._confidence);
        reversedStatement.truthValue = new TruthValue("1.00", "" + (fTimesC / (fTimesC + 1)));

        return reversedStatement;
    }

    //output indicators
    public static string INHERITANCE_COPULA = "-->";
    public static string SIMILARITY_COPULA = "<->";
    
    public static string INTENSIONAL_INTERSECTION_STRING = "|";
    public static string EXTENSIONAL_INTERSECTION_STRING = "&";
    public static string INTENSIONAL_DIFFERENCE_STRING = "-,";
    public static string EXTENSIONAL_DIFFERENCE_STRING = "~";
    public static string PRODUCT_STRING = "*";
    public static string INTENSIONAL_IMAGE_STRING = @"\";
    public static string EXTENSIONAL_IMAGE_STRING = "/";

    public static bool ContainsTermConnector(string statement)
    {
        return statement.Contains(INTENSIONAL_INTERSECTION_STRING) ||
            statement.Contains(EXTENSIONAL_INTERSECTION_STRING) ||
            statement.Contains(INTENSIONAL_DIFFERENCE_STRING) ||
            statement.Contains(EXTENSIONAL_DIFFERENCE_STRING) ||
            statement.Contains(PRODUCT_STRING) ||
            statement.Contains(INTENSIONAL_IMAGE_STRING) ||
            statement.Contains(EXTENSIONAL_IMAGE_STRING);

    }

    public static bool ContainsStatementCopula(string statement)
    {
        return statement.Contains(INHERITANCE_COPULA) ||
            statement.Contains(SIMILARITY_COPULA);

    }


}
