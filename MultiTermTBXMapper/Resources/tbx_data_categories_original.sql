-- phpMyAdmin SQL Dump
-- version 4.3.8
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Feb 20, 2017 at 03:15 PM
-- Server version: 5.6.32-78.1-log
-- PHP Version: 5.4.31

-- SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
-- SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `gevtermn_tbx_datacategories`
--

-- --------------------------------------------------------

--
-- Table structure for table `categories`
--

CREATE TABLE IF NOT EXISTS `categories` (
  `name` varchar(24) NOT NULL,
  `type` varchar(24) NOT NULL,
  `target` varchar(24) DEFAULT NULL,
  `element` varchar(24) DEFAULT NULL,
  `attribute` varchar(24) DEFAULT NULL,
  `level` varchar(24) DEFAULT NULL
);

--
-- Dumping data for table `categories`
--

INSERT INTO `categories` (`name`, `type`, `target`, `element`, `attribute`, `level`) VALUES
('begin paired tag', 'plainText', 'none', '<bpt>', '', ''),
('date', 'date (ISO format)', 'none', '<date>', '', ''),
('end paired tag', 'plainText', 'none', '<ept>', '', ''),
('foreign', 'noteText', 'none', '<foreign>', '', ''),
('highlight', 'plainText', 'element', '<hi>', '', ''),
('note', 'noteText', 'none', '<note>', '', ''),
('placeholder', 'plainText', 'none', '<ph>', '', ''),
('term', 'basicText', 'none', '<term>', '', ''),
('elementIdentifier', 'CDATA', '', '', 'id', ''),
('lang', 'language codes', '', '', 'xml:lang', ''),
('link', 'CDATA, IDREF', '', '', 'target', ''),
('audio', 'plainText', 'binaryData', '<descrip>', '', 'langSet, termEntry, term'),
('characteristic', 'plainText', 'none', '<descrip>', '', 'term'),
('conceptOrigin', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('context', 'noteText', 'none', '<descrip>', '', 'term'),
('contextType', 'picklist', 'none', '<descripNote>', '', 'langset, termEntry, term'),
('definition', 'noteText', 'none', '<descrip>', '', 'langSet, termEntry, term'),
('definitionType', 'picklist', 'element', '<descripNote>', '', 'langset, termEntry, term'),
('example', 'noteText', 'none', '<descrip>', '', 'langSet, termEntry, term'),
('explanation', 'noteText', 'none', '<descrip>', '', 'langSet, termEntry, term'),
('figure', 'plainText', 'binaryData', '<descrip>', '', 'langSet, termEntry, term'),
('otherBinaryData', 'plainText', 'binaryData', '<descrip>', '', 'langSet, termEntry, term'),
('quantity', 'plainText', 'none', '<descrip>', '', 'term'),
('range', 'plainText', 'none', '<descrip>', '', 'term'),
('sampleSentence', 'noteText', 'none', '<descrip>', '', 'term'),
('table', 'plainText', 'binaryData', '<descrip>', '', 'langSet, termEntry, term'),
('unit', 'plainText', 'none', '<descrip>', '', 'term'),
('video', 'plainText', 'binaryData', '<descrip>', '', 'langSet, termEntry, term'),
('antonymConcept', 'basicText', 'entry', '<descrip>', '', 'termEntry'),
('associatedConcept', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('broaderConceptGeneric', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('broaderConceptPartitive', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('conceptPosition', 'plainText', 'conceptSysDescrip', '<descrip>', '', 'langSet, termEntry'),
('coordinateConceptGeneric', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('coordinateConceptPartiti', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('relatedConcept', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('relatedConceptBroader', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('relatedConceptNarrower', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('sequentiallyRelatedConce', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('spatiallyRelatedConcept', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('subordinateConceptGeneri', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('subordinateConceptPartit', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('superordinateConceptGene', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('superordinateConceptPart', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('temporallyRelatedConcept', 'basicText', 'entry', '<descrip>', '', 'langSet, termEntry'),
('animacy', 'picklist', 'none', '<termNote>', '', 'term, termComponent'),
('etymology', 'noteText', 'none', '<termNote>', '', 'term, termComponent'),
('frequency', 'picklist', 'none', '<termNote>', '', 'term'),
('geographicalUsage', 'plainText', 'none', '<termNote>', '', 'term'),
('grammaticalGender', 'picklist', 'none', '<termNote>', '', 'term, termComponent'),
('grammaticalNumber', 'picklist', 'none', '<termNote>', '', 'term, termComponent'),
('grammaticalValency', 'plainText', 'none', '<termNote>', '', 'term'),
('language-planningQualifi', 'picklist', 'none', '<termNote>', '', 'term'),
('normativeAuthorization', 'picklist', 'none', '<termNote>', '', 'term'),
('partOfSpeech', 'plainText', 'none', '<termNote>', '', 'term, termComponent'),
('proprietaryRestriction', 'picklist', 'none', '<termNote>', '', 'term'),
('register', 'picklist', 'none', '<termNote>', '', 'term'),
('reliabilityCode', 'picklist', 'none', '<descrip>', '', 'langSet, termEntry, term'),
('temporalQualifier', 'picklist', 'none', '<termNote>', '', 'term'),
('termLocation', 'plainText', 'none', '<termNote>', '', 'term'),
('termProvenance', 'picklist', 'none', '<termNote>', '', 'term'),
('timeRestriction', 'plainText', 'none', '<termNote>', '', 'term'),
('transferComment', 'noteText', 'term', '<termNote>', '', 'term'),
('usageNote', 'noteText', 'none', '<termNote>', '', 'term'),
('abbreviatedFormFor', 'basicText', 'term', '<termNote>', '', 'term'),
('antonymTerm', 'basicText', 'term', '<termNote>', '', 'term'),
('directionality', 'picklist', 'term', '<termNote>', '', 'term'),
('falseFriend', 'basicText', 'term', '<termNote>', '', 'term'),
('homograph', 'basicText', 'term', '<termNote>', '', 'term'),
('shortFormFor', 'basicText', 'term', '<termNote>', '', 'term'),
('termType', 'picklist', 'none', '<termNote>', '', 'term'),
('hyphenation', 'plainText', 'none', '<termCompList>', '', 'termComponent'),
('lemma', 'plainText', 'none', '<termCompList>', '', 'termComponent'),
('lionHotkey', 'plainText', 'none', '<termNote>', '', 'term'),
('morphologicalElement', 'plainText', 'none', '<termCompList>', '', 'termComponent'),
('pronunciation', 'basicText', 'none', '<termNote>', '', 'term, termComponent'),
('syllabification', 'plainText', 'none', '<termCompList>', '', 'termComponent'),
('termElement', 'plainText', 'none', '<termCompList>', '', 'termComponent'),
('termStructure', 'plainText', 'none', '<termNote>', '', 'term, termComponent'),
('applicationSubset', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('businessUnitSubset', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('customerSubset', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('environmentSubset', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('productSubset', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('projectSubset', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('securitySubset', 'picklist', 'none', '<admin>', '', 'langset, termEntry, term'),
('subjectField', 'plainText', 'none', '<descrip>', '', 'termEntry'),
('subsetOwner', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('classificationCode', 'plainText', 'bibl', '<descrip>', '', 'langSet, termEntry, term'),
('indexHeading', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('keyword', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('searchTerm', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('sortKey', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('thesaurusDescriptor', 'plainText', 'thesaurusDescrip', '<descrip>', '', 'termEntry'),
('administrativeStatus', 'picklist', 'none', '<termNote>', '', 'term'),
('annotatedNote', 'noteText', 'none', '<admin>', '', 'langset, termEntry, term'),
('databaseType', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('domainExpert', 'plainText', 'bibl', '<admin>', '', 'langset, termEntry, term'),
('elementWorkingStatus', 'picklist', 'none', '<admin>', '', 'langset, termEntry, term'),
('entrySource', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('noteSource', 'plainText', 'none', '<adminNote>', '', 'langset, termEntry, term'),
('originatingDatabase', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('originatingInstitution', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('originatingPerson', 'plainText', 'none', '<admin>', '', 'langset, termEntry, term'),
('processStatus', 'picklist', 'none', '<termNote>', '', 'term'),
('responsibility', 'plainText', 'respPerson', '<transacNote>', '', 'langset, termEntry, term'),
('source', 'noteText', 'none', '<admin>', '', 'langset, termEntry, term'),
('sourceIdentifier', 'plainText', 'bibl', '<admin>', '', 'langset, termEntry, term'),
('sourceType', 'picklist', 'none', '<adminNote>', '', 'langset, termEntry, term'),
('transactionType', 'picklist', 'none', '<transac>', '', 'langset, termEntry, term'),
('usageCount', 'plainText', 'none', '<transacNote>', '', 'langset, termEntry, term'),
('corpusTrace', 'plainText', 'external', '<xref>', '', 'langset, termEntry, term'),
('crossReference', 'plainText', 'element', '<ref>', '', 'langset, termEntry, term'),
('externalCrossReference', 'plainText', 'external', '<xref>', '', 'langset, termEntry, term'),
('see', 'plainText', 'element', '<ref>', '', 'langset, termEntry, term'),
('xAudio', 'plainText', 'external', '<xref>', '', 'langset, termEntry, term'),
('xGraphic', 'plainText', 'external', '<xref>', '', 'langset, termEntry, term'),
('xMathML', 'plainText', 'external', '<xref>', '', 'langset, termEntry, term'),
('xSource', 'plainText', 'external', '<xref>', '', 'langset, termEntry, term'),
('xVideo', 'plainText', 'external', '<xref>', '', 'langset, termEntry, term'),
('bold', 'plainText', 'none', '<hi>', '', 'langset, termEntry, term'),
('entailedTerm', 'plainText', 'none', '<hi>', '', 'langset, termEntry, term'),
('hotkey', 'plainText', 'none', '<hi>', '', 'langset, termEntry, term'),
('italics', 'plainText', 'none', '<hi>', '', 'langset, termEntry, term'),
('math', 'plainText', 'none', '<hi>', '', 'langset, termEntry, term'),
('subscript', 'plainText', 'none', '<hi>', '', 'langset, termEntry, term'),
('superscript', 'plainText', 'none', '<hi>', '', 'langset, termEntry, term');

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
