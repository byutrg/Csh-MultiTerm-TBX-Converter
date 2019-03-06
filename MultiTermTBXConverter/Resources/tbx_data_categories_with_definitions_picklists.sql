BEGIN TRANSACTION;
CREATE TABLE "categories" (
	`id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`name`	varchar(36) NOT NULL,
	`type`	varchar(36) NOT NULL,
	`target`	varchar(36) DEFAULT NULL,
	`element`	varchar(36) DEFAULT NULL,
	`attribute`	varchar(36) DEFAULT NULL,
	`level`	varchar(36) DEFAULT NULL,
	`description`	TEXT NOT NULL
);
INSERT INTO `categories` VALUES (1,'begin paired tag','plainText','none','<bpt>','','',' The <bpt> element is used to delimit the first of a paired sequence of native codes (for example, an HTML <strong> tag). Each <bpt> shall have a corresponding <ept> element after it within the same parent element.');
INSERT INTO `categories` VALUES (2,'date','date (ISO format)','none','<date>','','','A date in ISO format. Permissible date values comply with ISO 8601 and shall use the format yyyy-mm-dd. The date element is used with terminology management data-categories in a <transacGrp> element. ');
INSERT INTO `categories` VALUES (3,'end paired tag','plainText','none','<ept>','','','End paired tag - The <ept> element is used to delimit the second of a paired sequence of native codes (for example, an HTML </strong> tag). Each <ept> shall have a corresponding <bpt> element before it within the same parent element. ');
INSERT INTO `categories` VALUES (4,'foreign','noteText','none','<foreign>','','','The <foreign> element is used to mark a segment of text that is in a different language from the surrounding text. The optional xml:lang attribute may be used to identify the language of the text contained in this element. ');
INSERT INTO `categories` VALUES (5,'highlight','plainText','element','<hi>','','','The <hi> element delimits a section of text. It can be used for various processing tasks. For example, it can be used to mark a mathematical expression, or an entailed term in a definition or other text field. The <hi> element is also allowed within <term> elements to mark portions of terms. For example, there are terms that have a character in subscript or superscript, or a component within the term that is italicized or requires other formatting.');
INSERT INTO `categories` VALUES (6,'note','noteText','none','<note>','','','Any kind of note. ');
INSERT INTO `categories` VALUES (7,'placeholder','plainText','none','<ph>','','','Placeholder - Used to encapsulate markup codes that do not occur in logical pairs with other codes. Examples include the XHTML <br/> and <hr/> tags. This element should not be used to encapsulate codes that occur in pairs (such as the XHTML <em> and </em> tags), which should instead be encapuslated with <bpt> and <ept>. ');
INSERT INTO `categories` VALUES (8,'term','basicText','none','<term>','','','The term that is being described in a <tig> or <ntig>. This element, as well as other term-like elements such as those mentioned in the table Types of terms, relations to terms in section 9, can contain a <hi> element to allow a limited amount of inline markup. This is intended to handle markup requirements in special cases such as may be required for terms that represent scientific concepts. However, it is strongly recommended to use inline markup only when necessary to represent the term in its base form. Do not use the <hi> element for presentational styles chosen for esthetic purposes.');
INSERT INTO `categories` VALUES (12,'audio','plainText','binaryData','<descrip>','','langSet, termEntry, term','A name or other identifier of an audio file: sound, spoken words, music, or other audible representations used to illustrate or explain terms or concepts. A <descrip> element that has a type attribute value of ''audio'' can also have a target attribute pointing to the ID of a <refObject> in the back matter of the TBX document instance that describes and points to the audio file. The element content shall be in plainText. 

NOTE If a direct pointer to the audio file is preferred within the <termEntry>, rather than pointing to the back matter, use an <xref> instead.');
INSERT INTO `categories` VALUES (13,'characteristic','plainText','none','<descrip>','','term','An abstraction of a property of an object or of a set of objects. For example, flammability is a characteristic of fuel.');
INSERT INTO `categories` VALUES (14,'conceptOrigin','plainText','none','<admin>','','langset, termEntry, term','The institution, region, community, culture, country, or other venue from which a concept covered by a terminological entry originates. Because this data-category provides information about the origin of the concept, it should occur at the <termEntry> level. The content shall be in plainText.');
INSERT INTO `categories` VALUES (15,'context','noteText','none','<descrip>','','term','A text which illustrates a concept or a term, by containing the concept designation itself. Contexts must be authentic, that is, they must be obtained from an existing source, and not created by the terminologist. Contexts are documented very frequently in terminology collections. Contexts can provide information for determining term usage and collocations. In TBX, the context as a term-related data-category. A context can be further categorized according to context type.');
INSERT INTO `categories` VALUES (16,'contextType','picklist','none','<descripNote>','','langset, termEntry, term','The characterization of a context according to a set of theoretical or pragmatic types. The content of the <descripNote> element when it has a type attribute value of ''contextType'' shall be one of the following:
	
	• definingContext
	• explanatoryContext
	• associativeContext
	• linguisticContext
	• metalinguisticContext
	• translatedContext
');
INSERT INTO `categories` VALUES (17,'definition','noteText','none','<descrip>','','langSet, termEntry, term','A representation of a concept by a descriptive statement which serves to differentiate that concept from related concepts.');
INSERT INTO `categories` VALUES (18,'definitionType','picklist','element','<descripNote>','','langset, termEntry, term','The characterization of a definition according to a set of theoretical or pragmatic types. The content of the <descripNote> element when it has a type attribute value of ''definitionType'' shall be one of the following:
	
	• intensionalDefinition
	• extensionalDefinition
	• partitiveDefinition
	• translatedDefinition
');
INSERT INTO `categories` VALUES (19,'example','noteText','none','<descrip>','','langSet, termEntry, term','A text which illustrates a concept or a term, by providing an example of the object designated by the concept or term, and not necessarily containing the concept designation itself. Some databases use example as a label for contextual references. The content of those data-categories should be converted to the data-category context for interchange purposes.');
INSERT INTO `categories` VALUES (20,'explanation','noteText','none','<descrip>','','langSet, termEntry, term','A statement that describes and clarifies a concept and makes it understandable, but does not necessarily differentiate it from other concepts.');
INSERT INTO `categories` VALUES (21,'figure','plainText','binaryData','<descrip>','','langSet, termEntry, term','A name or other description of a diagram, picture, or other graphic material used to illustrate a concept or group of concepts. A <descrip> element that has a type attribute value of ''figure'' can also have a target attribute pointing to the ID of a <refObject> in the back matter of the TBX document instance that describes and points to the file that contains the figure. The element content shall be in plainText.

NOTE If a direct pointer to the file that contains the figure is preferred within the <termEntry>, rather than pointing to the back matter, use an <xref> instead.');
INSERT INTO `categories` VALUES (22,'otherBinaryData','plainText','binaryData','<descrip>','','langSet, termEntry, term','Any binary data not covered by the categories figure, audio, video, and table. A <descrip> element that has a type attribute value of ''otherBinaryData'' can also have a target attribute pointing to the ID of a <refObject> in the back matter of the TBX document instance that describes and points to the binary file. The element content shall be in plainText. NOTE If a direct pointer to the binary file is preferred within the <termEntry>, rather than pointing to the back matter, use an <xref> instead.');
INSERT INTO `categories` VALUES (23,'quantity','plainText','none','<descrip>','','term','A measurable quantity associated with a concept. The element content shall be in plainText.');
INSERT INTO `categories` VALUES (24,'range','plainText','none','<descrip>','','term','The pair of limits within which a quantity is measured, as expressed by stating the lower and upper permissible values. The element content shall be in plainText.');
INSERT INTO `categories` VALUES (25,'sampleSentence','noteText','none','<descrip>','','term','A sentence composed (frequently by a terminologist or language planner) to illustrate the use of a term in cases where an authentic context is unavailable. Although contextual information should ideally be taken from actual texts written in the language in question, such contextual references may not exist for newly coined or suggested terms.');
INSERT INTO `categories` VALUES (26,'table','plainText','binaryData','<descrip>','','langSet, termEntry, term','The name of a table that is used to describe a concept. The table itself can be contained (or referred to as an external file) in a <refobject> in the back matter of a TBX document instance. The target attribute points to the <refobject> identifier. The element content shall be in plainText. NOTE If a direct pointer to the file that contains the table is preferred within the <termEntry>, rather than pointing to the back matter, use an <xref> instead.');
INSERT INTO `categories` VALUES (27,'unit','plainText','none','<descrip>','','term','A precisely-specified quantity in terms of which the magnitudes of other quantities can be stated, such as meter or volt. The element content shall be in plainText.');
INSERT INTO `categories` VALUES (28,'video','plainText','binaryData','<descrip>','','langSet, termEntry, term','A name or other description of recorded visual images used to represent or illustrate terminological information. A <descrip> element that has a type attribute value of ''video'' can also have a target attribute pointing to the ID of a <refObject> in the back matter of the TBX document instance that describes and points to the video file. The element content shall be in plainText. NOTE If a direct pointer to the video file is preferred within the <termEntry>, rather than pointing to the back matter, use an <xref> instead.');
INSERT INTO `categories` VALUES (29,'antonymConcept','basicText','entry','<descrip>','','termEntry','A term that designates a concept that is opposite in meaning to the concept represented by the entry containing the <descrip> element. A <descrip> element that has a type attribute value of ''antonymTerm'' can also have a target attribute pointing to the term ID of the term that represents the antonym concept. The display term in the content of this element shall be expressed in basicText.');
INSERT INTO `categories` VALUES (30,'associatedConcept','basicText','entry','<descrip>','','langSet, termEntry','A concept that exhibits a relation to the subject concept that can be established on the basis of non-hierarchical thematic connections. A <descrip> element that has a type attribute value of ''associatedConcept'' can also have a target attribute pointing to the entry ID of the <termEntry> that contains the associated concept. For example, teacher and school represent associated concepts.');
INSERT INTO `categories` VALUES (31,'broaderConceptGeneric','basicText','entry','<descrip>','','langSet, termEntry','A concept two or more levels of abstraction higher than subject concept in a generic hierarchical concept system. A <descrip> element that has a type attribute value of ''broaderConceptGeneric'' can also have a target attribute pointing to the entry ID of the <termEntry> that contains the broader concept. Generic relations between concepts are "is-a" relations. This means that the narrower concept "is an" instance of its broader generic concept. For example, a dog is a canine animal, and a canine animal is a mammal. Therefore, mammal is a broader generic concept for dog.');
INSERT INTO `categories` VALUES (32,'broaderConceptPartitive','basicText','entry','<descrip>','','langSet, termEntry','A concept two or more levels of abstraction higher than subject concept in a part-whole or meronymic hierarchical concept system. A <descrip> element that has a type attribute value of ''broaderConceptPartitive'' can also have a target attribute pointing to the entry ID of the <termEntry> that contains the broader concept. For example, a floor board is part of a house (broader partitive concept), but it can be more narrowly defined as being part of the floor (superordinate partitive concept), which in turn is part of the house. Partitive relations between concepts are "has-a" relations. This means that the broader partitive concept "has" its narrower concepts as parts of itself.');
INSERT INTO `categories` VALUES (33,'conceptPosition','plainText','conceptSysDescrip','<descrip>','','langSet, termEntry','The position of a concept in a concept system. A <descrip> element that has a type attribute value of ''conceptPosition'' can also have a target attribute pointing to the ID of a position in the concept system. The element content shall be in plainText.');
INSERT INTO `categories` VALUES (34,'coordinateConceptGeneric','basicText','entry','<descrip>','','langSet, termEntry','A concept having the same nearest superordinate concept as the concept being described by this <descrip> element in a generic concept system. A <descrip> element that has a type attribute value of ''coordinateConceptGeneric'' can also have a target attribute pointing to the ID of the coordinate concept.');
INSERT INTO `categories` VALUES (35,'coordinateConceptPartitive','basicText','entry','<descrip>','','langSet, termEntry','A concept having the same nearest superordinate concept as the concept being described by this <descrip> element in a partitive concept system. A <descrip> element that has a type attribute value of ''coordinateConceptPartitive'' can also have a target attribute pointing to the ID of the coordinate concept.');
INSERT INTO `categories` VALUES (36,'relatedConcept','basicText','entry','<descrip>','','langSet, termEntry','A concept that has an associative relation to another concept, such as teacher and school. A <descrip> element that has a type attribute value of ''relatedConcept'' can also have a target attribute pointing to the entry identifier of the related concept.');
INSERT INTO `categories` VALUES (37,'relatedConceptBroader','basicText','entry','<descrip>','','langSet, termEntry','A concept that is broader at some level with respect to a related or associated concept, but without comprising any reference to a generic or partitive system. A <descrip> element that has a type attribute value of ''relatedConceptBroader'' can also have a target attribute pointing to the entry identifier of the related concept.');
INSERT INTO `categories` VALUES (38,'relatedConceptNarrower','basicText','entry','<descrip>','','langSet, termEntry','A concept that is narrower at some level with respect to a related or associated concept, but without comprising any reference to a generic or partitive system. A <descrip> element that has a type attribute value of ''relatedConceptNarrower'' can also have a target attribute pointing to the entry identifier of the related concept.');
INSERT INTO `categories` VALUES (39,'sequentiallyRelatedConcept','basicText','entry','<descrip>','','langSet, termEntry','A concept that has a relation to the subject concept based on spatial or temporal proximity. Temporal relations and spatial relations can be sequential. A <descrip> element that has a type attribute value of ''sequentiallyRelatedConcept'' can also have a target attribute pointing to the entry identifier of the related concept.');
INSERT INTO `categories` VALUES (40,'spatiallyRelatedConcept','basicText','entry','<descrip>','','langSet, termEntry','A concept that has a relation of dependence with the subject concept, referring to objects based on their relative physical positions. A <descrip> element that has a type attribute value of ''spatiallyRelatedConcept'' can also have a target attribute pointing to the entry identifier of the related concept.');
INSERT INTO `categories` VALUES (41,'subordinateConceptGeneric','basicText','entry','<descrip>','','langSet, termEntry','A concept that has a generic relation with the subject concept, and a broader intension. A <descrip> element that has a type attribute value of ''subordinateConceptGeneric'' can also have a target attribute pointing to the entry identifier of the subordinate concept.');
INSERT INTO `categories` VALUES (42,'subordinateConceptPartitive','basicText','entry','<descrip>','','langSet, termEntry','A concept that has a partitive relation with the subject concept, and represents one of the parts of the subject concept. A <descrip> element that has a type attribute value of ''subordinateConceptPartitive'' can also have a target attribute pointing to the entry identifier of the subordinate concept.');
INSERT INTO `categories` VALUES (43,'temporallyRelatedConcept','basicText','entry','<descrip>','','langSet, termEntry','A concept related to the subject concept through a sequential relation involving events in time. A <descrip> element that has a type attribute value of ''temporallyRelatedConcept'' can also have a target attribute pointing to the entry identifier of the related concept.');
INSERT INTO `categories` VALUES (44,'animacy','picklist','none','<termNote>','','term, termComponent','The characteristic of a word indicating that in a given discourse community, its referent is considered to be alive or to possess a quality of volition or consciousness. The value of the <termNote> element when it has a type attribute value of animacy shall be one of: animate, inanimate, otherAnimacy.');
INSERT INTO `categories` VALUES (45,'etymology','noteText','none','<termNote>','','term, termComponent','Information on the origin of a word and the development of its meaning. The element content shall be expressed in noteText.');
INSERT INTO `categories` VALUES (46,'frequency','picklist','none','<termNote>','','term','The relative commonness with which a term occurs. The content of the <termNote> element when it has a type attribute value of ''frequency'' shall be one of: commonlyUsed, infrequentlyUsed, rarelyUsed. The assessment of frequency of a term can be based on subjective criteria, or it can reflect computer analysis of text corpora.');
INSERT INTO `categories` VALUES (47,'geographicalUsage','plainText','none','<termNote>','','term','A code identifying any specific geographical area where the term is used, for instance, to differentiate between UK English and US English. Preferred values are language codes combined with geographical codes, from IETF RFC 4646 or its successor, as identified in IETF BCP 47. For example: en-US for American English. The element content shall be expressed in plainText. ');
INSERT INTO `categories` VALUES (48,'grammaticalGender','picklist','none','<termNote>','','term, termComponent','The gender of the word. The value of the <termNote> element when it has a type attribute value of ''grammaticalGender'' shall be one of: masculine, feminine, neuter, or otherGender.');
INSERT INTO `categories` VALUES (49,'grammaticalNumber','picklist','none','<termNote>','','term, termComponent','A property of a term that indicates the number of objects referred to by the term. The content of the <termNote> element when it has a type attribute value of ''grammaticalNumber'' shall be one of:

	• singular
	• plural
	• dual
	• mass
	• otherNumber

For example, the terms rice and bread are mass nouns because there are no separate singular and plural forms. 
NOTE If the singular and the plural forms of a term do not designate the same concept, they should be recorded in separate entries.');
INSERT INTO `categories` VALUES (50,'grammaticalValency','plainText','none','<termNote>','','term','The range of syntactic elements either required or permitted by a lexical unit. The valency of eat includes a subject ("I" in "I am eating") and an object ("cheese" in "I am eating cheese".) It can thus be said to be "bivalent". The element content shall be expressed in plainText.');
INSERT INTO `categories` VALUES (51,'language-planningQualifier','picklist','none','<termNote>','','term','A qualifier assigned to a provisional term within a language planning or descriptive terminology environment. The content of the <termNote> element when it has a type attribute value of ''language-planningQualifier'' shall be one of:
	
	• recommendedTerm
	• nonstandardizedTerm
	• proposedTerm
	• newTerm');
INSERT INTO `categories` VALUES (52,'normativeAuthorization','picklist','none','<termNote>','','term','A term status qualifier assigned by an authoritative body, such as a standards body or a governmental entity with a regulatory function. The content of the <termNote> element when it has a type attribute value of ''normativeAuthorization'' shall be one of:
	
	• standardizedTerm
	• preferredTerm
	• admittedTerm
	• deprecatedTerm
	• supersededTerm
	• legalTerm
	• regulatedTerm

The name of the authoritative body should be documented in the terminology repository, and referenced in the TBX document instance by using a <refObject>. ');
INSERT INTO `categories` VALUES (53,'partOfSpeech','plainText','none','<termNote>','','term, termComponent','A category assigned to a word based on its grammatical and semantic properties. For example: noun, verb, or adjective. The element content shall be expressed in plainText, and preferably be limited to a set of picklist values. Only one value shall be permitted for each <term> in a given entry. For example, the following values are permitted in the TBX-Basic TML:
	
	• noun
	• verb
	• adjective
	• adverb
	• properNoun
	• other');
INSERT INTO `categories` VALUES (54,'proprietaryRestriction','picklist','none','<termNote>','','term','A restriction placed on a term for the purpose of protecting the right of an organization to the exclusive use of the
term. The content of the <termNote> element when it has a type attribute value of ''proprietaryRestriction'' shall be one of:
	
	• trademark
	• serviceMark
	• tradeName

NOTE The content value "copyright" is not included because copyright only applies to larger text units, such as books, not to terms.');
INSERT INTO `categories` VALUES (55,'register','picklist','none','<termNote>','','term','Classification indicating the level of language assigned to a term. The content of the <termNote> element when it has a type attribute value of ''register'' shall be one of:

	• colloquialRegister
	• neutralRegister
	• technicalRegister
	• in-houseRegister
	• bench-levelRegister
	• slangRegister
	• vulgarRegister');
INSERT INTO `categories` VALUES (56,'reliabilityCode','picklist','none','<descrip>','','langSet, termEntry, term','A code assigned to a data-category or record indicating accuracy and/or completeness. The content of the <descrip> element when it has a type attribute value of ''reliabilityCode'' shall be a value from 1 (least reliable) to 10 (most reliable).');
INSERT INTO `categories` VALUES (57,'temporalQualifier','picklist','none','<termNote>','','term','A property of a term with respect to its use over time. The content of the <termNote> element when it has a type attribute value of ''temporalQualifier'' shall be one of:

	• archaicTerm
	• outdatedTerm
	• obsoleteTerm');
INSERT INTO `categories` VALUES (58,'termLocation','plainText','none','<termNote>','','term','A location in a document, computer file, or other information medium, where the term frequently occurs, such as a user interface object (in software), a packaging element, a component in an industrial process, and so forth. The element content shall be expressed in plainText, and preferably be restricted to a set of picklist values. The following picklist values are recommended for software user interface locations in a Windows environment.

	• checkBox
	• comboBox
	• comboBoxElement
	• dialogBox
	• groupBox
	• informativeMessage
	• interactiveMessage
	• menuItem
	• progressBar
	• pushButton
	• radioButton
	• slider
	• spinBox
	• tab
	• tableText
	• textBox
	• toolTip
	• user-definedType');
INSERT INTO `categories` VALUES (59,'termProvenance','picklist','none','<termNote>','','term','Classification of a term according to the methodology employed in creating the term. The content of the <termNote> element when it has a type attribute value of ''termProvenance'' shall be one of:

	• transdisciplinaryBorrowing
	• translingualBorrowing
	• loanTranslation
	• neologism');
INSERT INTO `categories` VALUES (60,'timeRestriction','plainText','none','<termNote>','','term','The indication of a period of time during or since which a term was or has been subject to specified usage. For example, several European countries have redefined the requirements for certain university degrees in recent years. If, for instance, the requirements for a baccalaureate degree changed from three to four years in 1993, then any terminology entry defining the term used to designate this degree would have to specify the time restriction affecting the definition, which would be different before 1993 and after 1993. The element content shall be expressed in plainText. ');
INSERT INTO `categories` VALUES (61,'transferComment','noteText','term','<termNote>','','term','Note included in a term entry providing information on the degree of equivalence, directionality or other special features affecting equivalence between a term in one language and another term in a second language. This element can be used in conjunction with the <termNote type="directionality"> element to indicate the preferred translation of the term in question. The <termNote> element with a type attribute value of ''transferComment'' can also have a target attribute to point to the term that is the object of the transfer comment.');
INSERT INTO `categories` VALUES (62,'usageNote','noteText','none','<termNote>','','term','A note containing information on the usage of the term.');
INSERT INTO `categories` VALUES (63,'abbreviatedFormFor','basicText','term','<termNote>','','term','A link used to identify a relation between a term that is an abbreviated form, and its full or expanded form. It may be coupled with a target attribute to point to the entry of the full or expanded form. For example, the term WWW could have the following tag to point to its full form: <termNote type="abbreviatedFormFor" target="tidworldwideweb">World Wide Web</termNote>. The actual content of the element would be the text World Wide Web, which allows the user to see what term is the full form of the current term without having to go to the location of the full form in the file (the location would normally be somewhere within the current entry, since the abbreviated form and the full form represent the same concept). The element content shall be expressed in basicText. The identification of a relation between an abbreviated term and its full form indicates which term in a terminological entry is the full form of the abbreviated term. In many languages, English abbreviations are commonly used, but the English full form may be avoided in favor of an equivalent full form in the language in question. In this case, the English abbreviation and the target language full form will both appear in the target language section of the entry.');
INSERT INTO `categories` VALUES (64,'antonymTerm','basicText','term','<termNote>','','term','A term that designates a concept that is opposite in meaning to the concept represented by the entry containing the <descrip> element. A <descrip> element that has a type attribute value of ''antonymTerm'' can also have a target attribute pointing to the term ID of the term that represents the antonym concept. The display term in the content of this element shall be expressed in basicText.');
INSERT INTO `categories` VALUES (65,'directionality','picklist','term','<termNote>','','term','A data-category that, through the value of its target attribute, indicates the preferred translation of the term in the <term> element of the current <tig> from among the available translations in the entry. The target attribute should point to the identifier of the <tig> corresponding to the target term. In the case of the values monodirectional and bidirectional, the target term is the preferred term. The content of the <termNote> element when it has a type attribute value of ''directionality'' indicates the scope of directionality, and shall be one of the following:

	• monodirectional: The translation is only preferred going from the source language (current <tig> to the target language (target <tig>).
	• bidirectional: The translation is preferred in both directions, i.e. the term in the target <tig> is a preferred translation of the term in the current <tig> and vice-versa.
	• incommensurate: The relation between the term in the current <tig> and the term in the target <tig> as translations is imperfect and should be used with care. In this case, the concepts are not equal. A <termNote type="transferComment"> should be used to provide information about the difference between the concepts.
	• undetermined: The scope of directionality has not been determined.');
INSERT INTO `categories` VALUES (66,'falseFriend','basicText','term','<termNote>','','term','A term in another language that appears to be semantically or morphologically similar to the subject term in the entry, but that does not represent the same concept. For example, in French, the term réaliser means "to accomplish", and therefore, it is a false friend of the English term realize (i.e. to become conscious of). A <termNote> element with the type attribute value of ''falseFriend'' can also have a target attribute whose value is a pointer to a term in another entry. The element content shall be expressed in basicText.');
INSERT INTO `categories` VALUES (67,'homograph','basicText','term','<termNote>','','term','A word that is spelled like the subject term in the entry, but that has a different meaning, and origin. Homographs are terms that have the same written form, but are derived from different etymological origins, for example, a port for boats and a port for computer peripherals. A <termNote> element having a type attribute value of ''homograph'' can also have a target attribute to point to the <tig> of the entry that describes the homograph. The element content shall be expressed in basicText.');
INSERT INTO `categories` VALUES (68,'shortFormFor','basicText','term','<termNote>','','term','A linking element used to identify a relation between the subject term when it is a short form, and its full or expanded form. It may be coupled with a target attribute to point to the full or expanded form. Normally both the short form and the full form will be contained in the same entry, but each in its own <tig>, because they represent the same concept. For example, the term United States could have the following markup to point to the full form term United States of America: <termNote type="shortFormFor" target="tid-unitedstatesofamerica>United States of America</termNote>. The content of the element would be the target term, expressed in basicText, which allows the user to see the target term without going to its location in the entry. The identification of a relation between a short form and its full form indicates which term in a terminological entry is the full form of the short form.');
INSERT INTO `categories` VALUES (69,'termType','picklist','none','<termNote>','','term','A categorization of a term with respect to formal representations of terms or where or how they are used, for example, acronym. The content of the <termNote> element when it has a type attribute value of ''termType'' shall be one of the following:

	• abbreviation
	• acronym
	• clippedTerm
	• commonName
	• entryTerm
	• equation
	• formula
	• fullForm
	• initialism
	• internationalism
	• internationalScientificTerm
	• logicalExpression
	• partNumber
	• phraseologicalUnit
	• transcribedForm
	• transliteratedForm
	• shortForm
	• shortcut
	• sku
	• standardText
	• string
	• symbol
	• synonym
	• synonymousPhrase
	• variant

Refer to ISO 12620 for definitions of the above term types. The value shortcut refers to a string that acts as a
keyboard shortcut on computers, for example, Ctrl + V is a term that represents a keyboard shortcut for the concept
of paste.');
INSERT INTO `categories` VALUES (70,'hyphenation','plainText','none','<termCompList>','','termComponent','The <termComp> elements contain the parts of a term as it would be hyphenated, such as at the end of a line, according to a given set of rules.');
INSERT INTO `categories` VALUES (71,'lemma','plainText','none','<termCompList>','','termComponent','lexical unit chosen according to lexicographical conventions to represent the different forms of an inflectional paradigm [ISO 1951:2007]');
INSERT INTO `categories` VALUES (72,'lionHotkey','plainText','none','<termNote>','','term','(Localization hot key) The letter in a word that acts as a hotkey in a computer application. A hotkey is a key or a combination of keys on a computer keyboard that, when pressed at one time, performs a task (such as starting an application) as an alternative to using a mouse or other input device. Hotkeys are supported by many operating systems and applications. The hotkey is indicated by an ampersand in the term in software interface files, and by an underscore in the actual visual display. The element content shall be expressed in plainText.');
INSERT INTO `categories` VALUES (73,'morphologicalElement','plainText','none','<termCompList>','','termComponent','The <termComp> elements contain the individual morphemes that comprise the term. ');
INSERT INTO `categories` VALUES (74,'pronunciation','basicText','none','<termNote>','','term, termComponent','The representation of the manner by which a term is spoken. In TBX, pronunciation shall be represented in the International Phonetic Alphabet. A given term can have more than one pronunciation depending on the geographical area in which the term is used. In this case use a separate <tig> or <ntig> element for each pronunciation, and indicate the geographical area for each by using a <termNote> element with a type attribute value of ''geographicalUsage''. The element content shall be expressed in basicText.');
INSERT INTO `categories` VALUES (75,'syllabification','plainText','none','<termCompList>','','termComponent','The <termComp> elements contain the individual syllables that comprise the term.');
INSERT INTO `categories` VALUES (76,'termElement','plainText','none','<termCompList>','','termComponent','The <termComp> elements contain any other logically significant portion of the term.');
INSERT INTO `categories` VALUES (77,'termStructure','plainText','none','<termNote>','','term, termComponent','The structure of the components of a term, indicated using square brackets. For example, e.g. : [bank statement] [total] versus [bank] [statement total]. The element content shall be expressed in plainText. ');
INSERT INTO `categories` VALUES (78,'applicationSubset','plainText','none','<admin>','','langset, termEntry, term','An identifier that indicates that the parent element to which it applies is associated with a specific application. The content shall be in plainText.');
INSERT INTO `categories` VALUES (79,'businessUnitSubset','plainText','none','<admin>','','langset, termEntry, term','An identifier that indicates that the parent element to which it applies is associated with a specific department, division, or other unit of an enterprise. The content shall be in plainText.');
INSERT INTO `categories` VALUES (80,'customerSubset','plainText','none','<admin>','','langset, termEntry, term','An identifier that indicates that the parent element to which it applies is associated with a specific customer. The content shall be in plainText.');
INSERT INTO `categories` VALUES (81,'environmentSubset','plainText','none','<admin>','','langset, termEntry, term','An identifier that indicates a specific computer environment that the parent element is associated with. The content shall be in plainText.');
INSERT INTO `categories` VALUES (82,'productSubset','plainText','none','<admin>','','langset, termEntry, term','An identifier assigned to a term, record, or entry to indicate that it is related to or used in a product. The content shall be in plainText, and preferably one of a predefined set of picklist values.');
INSERT INTO `categories` VALUES (83,'projectSubset','plainText','none','<admin>','','langset, termEntry, term','An identifier assigned to a term, record, or entry to indicate that it is related to or used in a project. The content shall be in plainText, and preferably one of a predefined set of picklist values.');
INSERT INTO `categories` VALUES (84,'securitySubset','picklist','none','<admin>','','langset, termEntry, term','A classification of a term, record, or entry indicating that its distribution is restricted for security reasons. Sometimes, terms are not released to the public until the material that uses the term (product, service, etc.) is released to the public. Until that time, this element serves to protect the data from distribution. The element <admin> when its type attribute has the value ''securitySubset'' shall contain one of the following values:
	
	• public
	• confidential');
INSERT INTO `categories` VALUES (85,'subjectField','plainText','none','<descrip>','','termEntry','A field of special knowledge. Subject fields can be expressed in multiple levels, for example: Subject field (level 1): disease, Subject field (level 2): cancer, Subject field (level 3): non-Hodgkin''s lymphoma. Subject fields shall be expressed in plainText, and preferably be selected from a defined set of picklist values. Subject fields shall be specified at the concept (termEntry) level.');
INSERT INTO `categories` VALUES (86,'subsetOwner','plainText','none','<admin>','','langset, termEntry, term','The owner of the subset of terminological entries to which this entry belongs. The content shall be in plainText.');
INSERT INTO `categories` VALUES (87,'classificationCode','plainText','bibl','<descrip>','','langSet, termEntry, term','A set of symbols, with rules for their application, used to represent classes and their interrelations. A <descrip> element that has a type attribute value of ''classificationCode'' can also have a target attribute pointing to the ID of a bibliographic <refobject> in the back matter describing the classification code. The element content shall be in plainText. NOTE If a direct pointer to the classification code is preferred within the <termEntry>, rather than pointing to the back matter, use an <xref> instead.');
INSERT INTO `categories` VALUES (88,'indexHeading','plainText','none','<admin>','','langset, termEntry, term','A term chosen to be used in an index to represent an item or a concept in a document. The content shall be in plainText.');
INSERT INTO `categories` VALUES (89,'keyword','plainText','none','<admin>','','langset, termEntry, term','A word or group of words, possibly in lexicographically standardized (lemmatized) form, taken out of the text of a document, which characterizes the text and enables its retrieval. For example, the terms data-category and terminological entry are keywords for this international standard. In a terminological database that contains the terminology of ISO 30042, they could be identified as keywords by using this element. The content shall be in plainText.');
INSERT INTO `categories` VALUES (90,'searchTerm','plainText','none','<admin>','','langset, termEntry, term','A term entered in a term entry for purposes of retrieval. The content shall be in plainText');
INSERT INTO `categories` VALUES (91,'sortKey','plainText','none','<admin>','','langset, termEntry, term','A character string used for sorting and merging operations. A sort key can enable alphabetic or systematic access. The content shall be in plainText.');
INSERT INTO `categories` VALUES (92,'thesaurusDescriptor','plainText','thesaurusDescrip','<descrip>','','termEntry','The term in a thesaurus that can be used to represent a concept in a terminological entry. A <descrip> element that has a type attribute value of ''thesaurusDescriptor'' can also have a target attribute pointing to the ID of the <refObject> in the back matter of the TBX document instance that describes the thesaurus. The element content shall be in plainText. NOTE If a direct pointer to the thesaurus is preferred within the <termEntry>, rather than pointing to the back matter, use an <xref> instead.');
INSERT INTO `categories` VALUES (93,'administrativeStatus','picklist','none','<termNote>','','term','The status of a term within a certain working environment. The data-category administrativeStatus should be linked, through a target attribute, to a <refObject> in the back matter that identifies the administrative organization that determined the status. The element <termNote> when its type attribute has the value ''administrativeStatus'' shall contain one of the following values:

	• admittedTerm-admn-sts
	• deprecatedTerm-admn-sts
	• legalTerm-admn-sts
	• preferredTerm-admn-sts
	• regulatedTerm-admn-sts
	• standardizedTerm-admn-sts
	• supersededTerm-admn-sts

NOTE The status of terms with respect to a standardization process is covered by normativeAuthorization. ');
INSERT INTO `categories` VALUES (94,'annotatedNote','noteText','none','<admin>','','langset, termEntry, term','A note that can be coupled with an <adminNote> element to indicate the source of the note or other administrative information about the note. If no source or additional information about the note is required, use the <note> element.');
INSERT INTO `categories` VALUES (95,'databaseType','plainText','none','<admin>','','langset, termEntry, term','An identifier that indicates the type of database from which the parent element originated. This element should normally occur at the <termEntry> level because typically entire entries originate from or are imported from another database. The content shall be in plainText.');
INSERT INTO `categories` VALUES (96,'domainExpert','plainText','bibl','<admin>','','langset, termEntry, term','Subject field specialist who provides information for a terminological entry, such as a definition or target language equivalent. An <admin> element that has a type attribute value of ''domainExpert'' can also have a target attribute pointing to the identifier of a reference entry in the back matter of the TBX document instance that provides information about the domain expert. The content shall be in plainText.');
INSERT INTO `categories` VALUES (97,'elementWorkingStatus','picklist','none','<admin>','','langset, termEntry, term','A value indicating the level of completeness and accuracy of an element (field, record, entry) within a terminological collection. The element <admin> when its type attribute has the value ''elementWorkingStatus'' shall contain one of the following values:
	
	• starterElement
	• workingElement
	• consolidatedElement
	• archiveElement
	• importedElement
	• exportedElement');
INSERT INTO `categories` VALUES (98,'entrySource','plainText','none','<admin>','','langset, termEntry, term','A database or file from which the entry was imported. The content shall be in plainText.');
INSERT INTO `categories` VALUES (99,'noteSource','plainText','none','<adminNote>','','langset, termEntry, term','The source of a note that is expressed in a <admin> element having the type attribute value ''annotatedNote''.');
INSERT INTO `categories` VALUES (100,'originatingDatabase','plainText','none','<admin>','','langset, termEntry, term','The database from which the parent element originated. The content shall be in plainText.');
INSERT INTO `categories` VALUES (101,'originatingInstitution','plainText','none','<admin>','','langset, termEntry, term','An institution (i.e., company, government agency, etc.) that provided the information in the parent element. The content shall be in plainText.');
INSERT INTO `categories` VALUES (102,'originatingPerson','plainText','none','<admin>','','langset, termEntry, term','An individual who provided the information in the parent element. The content shall be in plainText.');
INSERT INTO `categories` VALUES (103,'processStatus','picklist','none','<termNote>','','term','The status of a term with respect to its advancement within the terminology management process. The content of the <termNote> element when it has a type attribute value of ''processStatus'' shall be one of: 

	• unprocessed
	• provisionallyProcessed
	• finalized');
INSERT INTO `categories` VALUES (104,'responsibility','plainText','respPerson','<transacNote>','','langset, termEntry, term','The name of a person or entity associated with a terminology management transaction. The <transacNote> element with a type attribute value of ''responsibility'' can also have a target attribute to point to the identifier of a bibliographic entry in a <refObject> that contains information about the person or entity.');
INSERT INTO `categories` VALUES (105,'source','noteText','none','<admin>','','langset, termEntry, term','A complete citation of the source of the information in the parent element. ');
INSERT INTO `categories` VALUES (106,'sourceIdentifier','plainText','bibl','<admin>','','langset, termEntry, term','A pointer to a bibliographic entry that contains information about the source of the parent element. An <admin> element that has a type attribute value of ''sourceIdentifier'' also has a target attribute containing the identifier of the bibliographic entry. The content of the element is a display value for the source.');
INSERT INTO `categories` VALUES (107,'sourceType','picklist','none','<adminNote>','','langset, termEntry, term','A property describing the source text, i.e. whether it is actual parallel text (text that is equivalent to text in another language) or background material. The content of the <adminNote> element when it has a type attribute value of ''sourceType'' shall be one of the following:
	
	• parallelText
	• backgroundText');
INSERT INTO `categories` VALUES (108,'transactionType','picklist','none','<transac>','','langset, termEntry, term','A record that indicates the stage of the entry within the overall process of creation, approval, and use of a terminology entry.');
INSERT INTO `categories` VALUES (109,'usageCount','plainText','none','<transacNote>','','langset, termEntry, term','The number of times an entry has been accessed.');
INSERT INTO `categories` VALUES (110,'corpusTrace','plainText','external','<xref>','','langset, termEntry, term','A notation indicating the location of an external contextual resource.');
INSERT INTO `categories` VALUES (111,'crossReference','plainText','element','<ref>','','langset, termEntry, term','A pointer to another related location, such as another entry or another term. The <ref> element with a type attribute value of ''crossReference'' shall also have a target attribute to point to the identifier of the entry or term.');
INSERT INTO `categories` VALUES (112,'externalCrossReference','plainText','external','<xref>','','langset, termEntry, term','A term or other terminological resource that is external to the terminological database or TBX document instance.');
INSERT INTO `categories` VALUES (113,'see','plainText','element','<ref>','','langset, termEntry, term','An element that points to a location where additional information can be found. The <ref> element with a type attribute value of ''see'' shall also have a target attribute to point to the location of the information.');
INSERT INTO `categories` VALUES (114,'xAudio','plainText','external','<xref>','','langset, termEntry, term','External audio file.');
INSERT INTO `categories` VALUES (115,'xGraphic','plainText','external','<xref>','','langset, termEntry, term','External graphic file.');
INSERT INTO `categories` VALUES (116,'xMathML','plainText','external','<xref>','','langset, termEntry, term','A mathematical concept expressed in MathML.');
INSERT INTO `categories` VALUES (117,'xSource','plainText','external','<xref>','','langset, termEntry, term','An external source of information such as the source of a definition or context sentence.');
INSERT INTO `categories` VALUES (118,'xVideo','plainText','external','<xref>','','langset, termEntry, term','External video file. ');
INSERT INTO `categories` VALUES (119,'bold','plainText','none','<hi>','','langset, termEntry, term','Indicates that the content enclosed in the <hi> element is rendered in boldface.');
INSERT INTO `categories` VALUES (120,'entailedTerm','plainText','none','<hi>','','langset, termEntry, term','A term, found in the content of a text element such as <descrip type="definition">, that is defined in another terminological entry in the same terminological collection.');
INSERT INTO `categories` VALUES (121,'hotkey','plainText','none','<hi>','','langset, termEntry, term','A key or a combination of keys on a computer keyboard that, when pressed at one time, performs a task (such as starting an application) as an alternative to using a mouse or other input device. Hotkeys are supported by many operating systems and applications (the hotkey is indicated by an ampersand in the term and by an underscore in the visual display). This type of <hi> element allows a hotkey to be identified as such in a text field such as a context sentence. To identify a hotkey for the head term of an entry, use the lionHotkey value of the type attribute on a <termNote>.');
INSERT INTO `categories` VALUES (122,'italics','plainText','none','<hi>','','langset, termEntry, term','Indicates that the content enclosed in the <hi> element is rendered in italics.');
INSERT INTO `categories` VALUES (123,'math','plainText','none','<hi>','','langset, termEntry, term','Indicates that the content enclosed in the <hi> element is mathematical in nature.');
INSERT INTO `categories` VALUES (124,'subscript','plainText','none','<hi>','','langset, termEntry, term','Indicates that the content enclosed in the <hi> element is rendered in subscript.');
INSERT INTO `categories` VALUES (125,'superscript','plainText','none','<hi>','','langset, termEntry, term','Indicates that the content enclosed in the <hi> element is rendered in superscript.');
INSERT INTO `categories` VALUES (126,'relatedTerm','plainText','term','<ref>','','term','A term connected to another term by a coordinate or associative relation.');

CREATE TABLE "picklists" (
	`id`	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	`value`	TEXT NOT NULL,
	`description`	TEXT,
	`category_id`	INTEGER NOT NULL
);
INSERT INTO `picklists` VALUES (1,'parallelText',NULL,107);
INSERT INTO `picklists` VALUES (2,'backgroundText',NULL,107);
INSERT INTO `picklists` VALUES (3,'starterElement',NULL,97);
INSERT INTO `picklists` VALUES (4,'workingElement',NULL,97);
INSERT INTO `picklists` VALUES (5,'consolidatedElement',NULL,97);
INSERT INTO `picklists` VALUES (6,'archiveElement',NULL,97);
INSERT INTO `picklists` VALUES (7,'importedElement',NULL,97);
INSERT INTO `picklists` VALUES (8,'admittedTerm-admn-sts',NULL,93);
INSERT INTO `picklists` VALUES (9,'deprecatedTerm-admn-sts',NULL,93);
INSERT INTO `picklists` VALUES (10,'legalTerm-admn-sts',NULL,93);
INSERT INTO `picklists` VALUES (11,'preferredTerm-admn-sts',NULL,93);
INSERT INTO `picklists` VALUES (12,'regulatedTerm-admn-sts',NULL,93);
INSERT INTO `picklists` VALUES (13,'standardizedTerm-admn-sts',NULL,93);
INSERT INTO `picklists` VALUES (14,'supersededTerm-admn-sts',NULL,93);
INSERT INTO `picklists` VALUES (15,'public',NULL,84);
INSERT INTO `picklists` VALUES (16,'confidential',NULL,84);
INSERT INTO `picklists` VALUES (17,'abbreviation',NULL,69);
INSERT INTO `picklists` VALUES (18,'acronym',NULL,69);
INSERT INTO `picklists` VALUES (19,'clippedTerm',NULL,69);
INSERT INTO `picklists` VALUES (20,'commonName',NULL,69);
INSERT INTO `picklists` VALUES (21,'monodirectional','The translation is only preferred going from the source language (current <tig> to the target language (target <tig>).',65);
INSERT INTO `picklists` VALUES (22,'bidirectional','The translation is preferred in both directions, i.e. the term in the target <tig> is a preferred translation of the term in the current <tig> and vice-versa.',65);
INSERT INTO `picklists` VALUES (23,'incommensurate','The relation between the term in the current <tig> and the term in the target <tig> as translations is imperfect and should be used with care. In this case, the concepts are not equal. A <termNote type="transferComment"> should be used to provide information about the difference between the concepts.',65);
INSERT INTO `picklists` VALUES (24,'undetermined','The scope of directionality has not been determined.',65);
INSERT INTO `picklists` VALUES (25,'checkBox',NULL,58);
INSERT INTO `picklists` VALUES (26,'comboBox',NULL,58);
INSERT INTO `picklists` VALUES (27,'comboBoxElement',NULL,58);
INSERT INTO `picklists` VALUES (28,'dialogBox',NULL,58);
INSERT INTO `picklists` VALUES (29,'groupBox',NULL,58);
INSERT INTO `picklists` VALUES (30,'informativeMessage',NULL,58);
INSERT INTO `picklists` VALUES (31,'interactiveMessage',NULL,58);
INSERT INTO `picklists` VALUES (32,'menuItem',NULL,58);
INSERT INTO `picklists` VALUES (33,'progressBar',NULL,58);
INSERT INTO `picklists` VALUES (34,'pushButton',NULL,58);
INSERT INTO `picklists` VALUES (35,'radioButton',NULL,58);
INSERT INTO `picklists` VALUES (36,'slider',NULL,58);
INSERT INTO `picklists` VALUES (37,'spinBox',NULL,58);
INSERT INTO `picklists` VALUES (38,'tab',NULL,58);
INSERT INTO `picklists` VALUES (39,'tableText',NULL,58);
INSERT INTO `picklists` VALUES (40,'textBox',NULL,58);
INSERT INTO `picklists` VALUES (41,'toolTip',NULL,58);
INSERT INTO `picklists` VALUES (42,'user-definedType',NULL,58);
INSERT INTO `picklists` VALUES (43,'colloquialRegister',NULL,55);
INSERT INTO `picklists` VALUES (44,'neutralRegister',NULL,55);
INSERT INTO `picklists` VALUES (45,'technicalRegister',NULL,55);
INSERT INTO `picklists` VALUES (46,'in-houseRegister',NULL,55);
INSERT INTO `picklists` VALUES (47,'bench-levelRegister',NULL,55);
INSERT INTO `picklists` VALUES (48,'slangRegister',NULL,55);
INSERT INTO `picklists` VALUES (49,'vulgarRegister',NULL,55);
INSERT INTO `picklists` VALUES (50,'noun',NULL,53);
INSERT INTO `picklists` VALUES (51,'verb',NULL,53);
INSERT INTO `picklists` VALUES (52,'adjective',NULL,53);
INSERT INTO `picklists` VALUES (53,'adverb',NULL,53);
INSERT INTO `picklists` VALUES (54,'properNoun',NULL,53);
INSERT INTO `picklists` VALUES (55,'other',NULL,53);
INSERT INTO `picklists` VALUES (56,'standardizedTerm',NULL,52);
INSERT INTO `picklists` VALUES (57,'preferredTerm',NULL,52);
INSERT INTO `picklists` VALUES (58,'admittedTerm',NULL,52);
INSERT INTO `picklists` VALUES (59,'deprecatedTerm',NULL,52);
INSERT INTO `picklists` VALUES (60,'supersededTerm',NULL,52);
INSERT INTO `picklists` VALUES (61,'intensionalDefinition',NULL,18);
INSERT INTO `picklists` VALUES (62,'extensionalDefinition',NULL,18);
INSERT INTO `picklists` VALUES (63,'partitiveDefinition',NULL,18);
INSERT INTO `picklists` VALUES (64,'translatedDefinition',NULL,18);
INSERT INTO `picklists` VALUES (65,'definingContext',NULL,16);
INSERT INTO `picklists` VALUES (66,'explanatoryContext',NULL,16);
INSERT INTO `picklists` VALUES (67,'associativeContext',NULL,16);
INSERT INTO `picklists` VALUES (68,'linguisticContext',NULL,16);
INSERT INTO `picklists` VALUES (69,'metalinguisticContext',NULL,16);
INSERT INTO `picklists` VALUES (70,'translatedContext',NULL,16);
INSERT INTO `picklists` VALUES (71,'recommendedTerm',NULL,51);
INSERT INTO `picklists` VALUES (72,'nonstandardizedTerm',NULL,51);
INSERT INTO `picklists` VALUES (73,'proposedTerm',NULL,51);
INSERT INTO `picklists` VALUES (74,'newTerm',NULL,51);
INSERT INTO `picklists` VALUES (75,'trademark',NULL,54);
INSERT INTO `picklists` VALUES (76,'serviceMark',NULL,54);
INSERT INTO `picklists` VALUES (77,'tradeName',NULL,54);
INSERT INTO `picklists` VALUES (78,'archaicTerm',NULL,57);
INSERT INTO `picklists` VALUES (79,'outdatedTerm',NULL,57);
INSERT INTO `picklists` VALUES (80,'obsoleteTerm',NULL,57);
INSERT INTO `picklists` VALUES (81,'transdisciplinaryBorrowing',NULL,59);
INSERT INTO `picklists` VALUES (82,'translingualBorrowing',NULL,59);
INSERT INTO `picklists` VALUES (83,'loanTranslation',NULL,59);
INSERT INTO `picklists` VALUES (84,'neologism',NULL,59);
INSERT INTO `picklists` VALUES (85,'unprocessed',NULL,103);
INSERT INTO `picklists` VALUES (86,'provisionallyProcessed',NULL,103);
INSERT INTO `picklists` VALUES (87,'finalized',NULL,103);
INSERT INTO `picklists` VALUES (88,'singular',NULL,49);
INSERT INTO `picklists` VALUES (89,'plural',NULL,49);
INSERT INTO `picklists` VALUES (90,'dual',NULL,49);
INSERT INTO `picklists` VALUES (91,'mass',NULL,49);
INSERT INTO `picklists` VALUES (92,'otherNumber',NULL,49);
INSERT INTO `picklists` VALUES (93,'entryTerm',NULL,69);
INSERT INTO `picklists` VALUES (94,'equation',NULL,69);
INSERT INTO `picklists` VALUES (95,'formula',NULL,69);
INSERT INTO `picklists` VALUES (96,'fullForm',NULL,69);
INSERT INTO `picklists` VALUES (97,'initialism',NULL,69);
INSERT INTO `picklists` VALUES (98,'internationalism',NULL,69);
INSERT INTO `picklists` VALUES (99,'internationalScientificTerm',NULL,69);
INSERT INTO `picklists` VALUES (100,'logicalExpression',NULL,69);
INSERT INTO `picklists` VALUES (101,'partNumber',NULL,69);
INSERT INTO `picklists` VALUES (102,'phraseologicalUnit',NULL,69);
INSERT INTO `picklists` VALUES (103,'transcribedForm',NULL,69);
INSERT INTO `picklists` VALUES (104,'transliteratedForm',NULL,69);
INSERT INTO `picklists` VALUES (105,'shortForm',NULL,69);
INSERT INTO `picklists` VALUES (106,'shortcut',NULL,69);
INSERT INTO `picklists` VALUES (107,'sku',NULL,69);
INSERT INTO `picklists` VALUES (108,'standardText',NULL,69);
INSERT INTO `picklists` VALUES (109,'string',NULL,69);
INSERT INTO `picklists` VALUES (110,'symbol',NULL,69);
INSERT INTO `picklists` VALUES (111,'synonym',NULL,69);
INSERT INTO `picklists` VALUES (112,'synonymousPhrase',NULL,69);
INSERT INTO `picklists` VALUES (113,'variant',NULL,69);
INSERT INTO `picklists` VALUES (114, 'masculine', NULL, 48);
INSERT INTO `picklists` VALUES (115, 'feminine', NULL, 48);
INSERT INTO `picklists` VALUES (116, 'neuter', NULL, 48);
INSERT INTO `picklists` VALUES (117, 'otherGender', NULL, 48);
COMMIT;
