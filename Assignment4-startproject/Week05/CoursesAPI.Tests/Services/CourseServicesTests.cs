using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;
using CoursesAPI.Services.Services;
using CoursesAPI.Tests.MockObjects;
using CoursesAPI.Tests.TestExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoursesAPI.Tests.Services
{
	[TestClass]
	public class CourseServicesTests
	{
		private MockUnitOfWork<MockDataContext> _mockUnitOfWork;
		private CoursesServiceProvider _service;
		private List<TeacherRegistration> _teacherRegistrations;

		private const string SSN_DABS    = "1203735289";
		private const string SSN_GUNNA   = "1234567890";
		private const string INVALID_SSN = "9876543210";

		private const string NAME_GUNNA  = "Guðrún Guðmundsdóttir";
        private const string NAME_DABS   = "Daníel B. Sigurgeirsson";

        private const int COURSEID_VEFT_20153 = 1337;
		private const int COURSEID_VEFT_20163 = 1338;
        private const int COURSEID_PROG_20153 = 1555;
        private const int COURSEID_TGRA_20153 = 3333;
        private const int COURSEID_TGRA_20157 = 1733;
		private const int INVALID_COURSEID    = 9999;

        private const string COURSE_TEMPLID_VEFT = "T-514-VEFT";
        private const string COURSE_TEMPLID_PROG = "T-111-PROG";
        private const string COURSE_TEMPLID_TGRA = "T-511-TGRA";

        private const string COURSE_NAME_VEFT = "Vefþjónustur";
        private const string COURSE_NAME_PROG = "Forritun";
        private const string COURSE_NAME_TGRA = "Tölvugrafík";

        [TestInitialize]
		public void Setup()
		{
			_mockUnitOfWork = new MockUnitOfWork<MockDataContext>();

			#region Persons
			var persons = new List<Person>
			{
				// Of course I'm the first person,
				// did you expect anything else?
				new Person
				{
					ID    = 1,
					Name  = NAME_DABS,
					SSN   = SSN_DABS,
					Email = "dabs@ru.is"
				},
				new Person
				{
					ID    = 2,
					Name  = NAME_GUNNA,
					SSN   = SSN_GUNNA,
					Email = "gunna@ru.is"
				}
			};
			#endregion

			#region Course templates

			var courseTemplates = new List<CourseTemplate>
			{
				new CourseTemplate
				{
					CourseID    = COURSE_TEMPLID_VEFT,
					Description = "Í þessum áfanga verður fjallað um vefþj...",
					Name        = COURSE_NAME_VEFT
				},
                new CourseTemplate
                {
                    CourseID    = COURSE_TEMPLID_PROG,
                    Description = "Í þessum áfanga verður fjallað um grunngildi forr...",
                    Name        = COURSE_NAME_PROG
                },
                new CourseTemplate
                {
                    CourseID    = COURSE_TEMPLID_TGRA,
                    Description = "Í þessum áfagna verður fjallað um tölvug...",
                    Name        = COURSE_NAME_TGRA

                }
			};
			#endregion

			#region Courses
			var courses = new List<CourseInstance>
			{
				new CourseInstance
				{
					ID         = COURSEID_VEFT_20153,
					CourseID   = COURSE_TEMPLID_VEFT,
					SemesterID = "20153"
				},
				new CourseInstance
				{
					ID         = COURSEID_VEFT_20163,
					CourseID   = COURSE_TEMPLID_VEFT,
					SemesterID = "20163"
				},
                new CourseInstance
                {
                    ID         = COURSEID_PROG_20153,
                    CourseID   = COURSE_TEMPLID_PROG,
                    SemesterID = "20153"
                },
                new CourseInstance
                {
                    ID         = COURSEID_TGRA_20153,
                    CourseID   = COURSE_TEMPLID_TGRA,
                    SemesterID = "20153"
                },
                new CourseInstance
                {
                    ID         = COURSEID_TGRA_20157,
                    CourseID   = COURSE_TEMPLID_TGRA,
                    SemesterID = "20173"
                }
			};
			#endregion

			#region Teacher registrations
			_teacherRegistrations = new List<TeacherRegistration>
			{
				new TeacherRegistration
				{
					ID               = 101,
					CourseInstanceID = COURSEID_VEFT_20153,
					SSN              = SSN_DABS,
					Type             = TeacherType.MainTeacher
				},
                new TeacherRegistration
                {
                    ID               = 201,
                    CourseInstanceID = COURSEID_TGRA_20153,
                    SSN              = SSN_GUNNA,
                    Type             = TeacherType.MainTeacher
                }
			};
			#endregion

			_mockUnitOfWork.SetRepositoryData(persons);
			_mockUnitOfWork.SetRepositoryData(courseTemplates);
			_mockUnitOfWork.SetRepositoryData(courses);
			_mockUnitOfWork.SetRepositoryData(_teacherRegistrations);

			// TODO: this would be the correct place to add 
			// more mock data to the mockUnitOfWork!

			_service = new CoursesServiceProvider(_mockUnitOfWork);
		}

		#region GetCoursesBySemester
		/// <summary>
		/// TODO: implement this test, and several others!
		/// </summary>
		[TestMethod]
		public void GetCoursesBySemester_ReturnsEmptyListWhenNoDataDefined()
		{
            // Arrange:
            MockUnitOfWork<MockDataContext> _mockUnitOfWorkWithNoDataDefined = new MockUnitOfWork<MockDataContext>();
            CoursesServiceProvider _serviceWithNoDataDefined = new CoursesServiceProvider(_mockUnitOfWorkWithNoDataDefined);
            const string SEMESTER = "20153";

			// Act:
            var result = _serviceWithNoDataDefined.GetCourseInstancesBySemester(SEMESTER);

            // Assert:
            Assert.AreEqual(0, result.Count);
		}

		[TestMethod]
        public void GetCoursesBySemester_ReturnsAllCoursesOnAGivenSemester()
        {
            // Arrange:
            const string SEMESTER1 = "20153";
            const string SEMESTER2 = "20163";

            // Act:
            var result1 = _service.GetCourseInstancesBySemester(SEMESTER1);
            var result2 = _service.GetCourseInstancesBySemester(SEMESTER2);

            // Assert: 
            // Assert that the function returns all courses
            // on a given semester (no more, no less):
            Assert.AreEqual(3, result1.Count);
            Assert.AreNotEqual(2, result1.Count);
            Assert.AreNotEqual(4, result1.Count);
            Assert.AreEqual(1, result2.Count);
            Assert.AreNotEqual(0, result2.Count);
            Assert.AreNotEqual(2, result2.Count);
            // Assert that the function actually returns the correct courses:
            Assert.AreEqual(COURSEID_VEFT_20163, result2[0].CourseInstanceID);
            Assert.AreEqual(COURSE_TEMPLID_VEFT, result2[0].TemplateID);
            Assert.AreEqual(COURSE_NAME_VEFT, result2[0].Name);
        }

        [TestMethod]
        public void GetCoursesBySemester_WithNoSemesterDefined()
        {
            // Arrange:
            const string SEMESTER = null;

            // Act:
            var result = _service.GetCourseInstancesBySemester(SEMESTER);

            // Assert:
            // Assert that the number of courses are the same.
            Assert.AreEqual(3, result.Count);
            // Assert that all elements in the result array have the right attributes.
            Assert.AreEqual(COURSEID_VEFT_20153, result[0].CourseInstanceID);
            Assert.AreEqual(COURSE_TEMPLID_VEFT, result[0].TemplateID);
            Assert.AreEqual(COURSE_NAME_VEFT, result[0].Name);
            Assert.AreEqual(COURSEID_PROG_20153, result[1].CourseInstanceID);
            Assert.AreEqual(COURSE_TEMPLID_PROG, result[1].TemplateID);
            Assert.AreEqual(COURSE_NAME_PROG, result[1].Name);
            Assert.AreEqual(COURSEID_TGRA_20153, result[2].CourseInstanceID);
            Assert.AreEqual(COURSE_TEMPLID_TGRA, result[2].TemplateID);
            Assert.AreEqual(COURSE_NAME_TGRA, result[2].Name);
        }

        [TestMethod]
        // (10%) For each course returned, the name of the main teacher of the course 
        // should be included(see the definition of CourseInstanceDTO).
        public void GetCoursesBySemester_MainTeacherOfCourseIsIncluded()
        {
            //Arrange:
            const string SEMESTER = "20153";

            //Act:
            var result = _service.GetCourseInstancesBySemester(SEMESTER);
            var veft = result[0];
            var tgra = result[2];

            //Assert:
            Assert.AreEqual(NAME_DABS, veft.MainTeacher);
            Assert.AreEqual(NAME_GUNNA, tgra.MainTeacher);
        }

        [TestMethod]
        public void GetCoursesBySemester_MainTeacherHasNotBeenDefined()
        {
            // Arrange:
            const string SEMESTER_2017 = "20173";
            const string SEMESTER_2016 = "20163";

            // Act:
            var result_2017 = _service.GetCourseInstancesBySemester(SEMESTER_2017);
            var result_2016 = _service.GetCourseInstancesBySemester(SEMESTER_2016);

            // Assert:
            // Assert that the course id's is correct.
            Assert.AreEqual(COURSEID_TGRA_20157, result_2017[0].CourseInstanceID);
            Assert.AreEqual(COURSEID_VEFT_20163, result_2016[0].CourseInstanceID);
            // Assert that the main teacher of those two course are the empty string.
            Assert.AreEqual("", result_2017[0].MainTeacher);
            Assert.AreEqual("", result_2016[0].MainTeacher);
        }

        [TestMethod]
        public void GetCoursesBySemester_ble()
        {

        }

		#endregion

		#region AddTeacher

		/// <summary>
		/// Adds a main teacher to a course which doesn't have a
		/// main teacher defined already (see test data defined above).
		/// </summary>
		[TestMethod]
		public void AddTeacher_WithValidTeacherAndCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.MainTeacher
			};
			var prevCount = _teacherRegistrations.Count;
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			var dto = _service.AddTeacherToCourse(COURSEID_VEFT_20163, model);

			// Assert:

			// Check that the dto object is correctly populated:
			Assert.AreEqual(SSN_GUNNA, dto.SSN);
			Assert.AreEqual(NAME_GUNNA, dto.Name);

			// Ensure that a new entity object has been created:
			var currentCount = _teacherRegistrations.Count;
			Assert.AreEqual(prevCount + 1, currentCount);

			// Get access to the entity object and assert that
			// the properties have been set:
			var newEntity = _teacherRegistrations.Last();
			Assert.AreEqual(COURSEID_VEFT_20163, newEntity.CourseInstanceID);
			Assert.AreEqual(SSN_GUNNA, newEntity.SSN);
			Assert.AreEqual(TeacherType.MainTeacher, newEntity.Type);

			// Ensure that the Unit Of Work object has been instructed
			// to save the new entity object:
			Assert.IsTrue(_mockUnitOfWork.GetSaveCallCount() > 0);
		}

		[TestMethod]
		[ExpectedException(typeof(AppObjectNotFoundException))]
		public void AddTeacher_InvalidCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.AssistantTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			_service.AddTeacherToCourse(INVALID_COURSEID, model);
		}

		/// <summary>
		/// Ensure it is not possible to add a person as a teacher
		/// when that person is not registered in the system.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof (AppObjectNotFoundException))]
		public void AddTeacher_InvalidTeacher()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = INVALID_SSN,
				Type = TeacherType.MainTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			_service.AddTeacherToCourse(COURSEID_VEFT_20163, model);
		}

		/// <summary>
		/// In this test, we test that it is not possible to
		/// add another main teacher to a course, if one is already
		/// defined.
		/// </summary>
		[TestMethod]
		[ExpectedExceptionWithMessage(typeof (AppValidationException), "COURSE_ALREADY_HAS_A_MAIN_TEACHER")]
		public void AddTeacher_AlreadyWithMainTeacher()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_GUNNA,
				Type = TeacherType.MainTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			_service.AddTeacherToCourse(COURSEID_VEFT_20153, model);
		}

		/// <summary>
		/// In this test, we ensure that a person cannot be added as a
		/// teacher in a course, if that person is already registered
		/// as a teacher in the given course.
		/// </summary>
		[TestMethod]
		[ExpectedExceptionWithMessage(typeof (AppValidationException), "PERSON_ALREADY_REGISTERED_TEACHER_IN_COURSE")]
		public void AddTeacher_PersonAlreadyRegisteredAsTeacherInCourse()
		{
			// Arrange:
			var model = new AddTeacherViewModel
			{
				SSN  = SSN_DABS,
				Type = TeacherType.AssistantTeacher
			};
			// Note: the method uses test data defined in [TestInitialize]

			// Act:
			_service.AddTeacherToCourse(COURSEID_VEFT_20153, model);
		}

		#endregion
	}
}
