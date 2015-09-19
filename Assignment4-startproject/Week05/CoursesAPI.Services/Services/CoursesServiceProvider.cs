using System.Collections.Generic;
using System.Linq;
using CoursesAPI.Models;
using CoursesAPI.Services.DataAccess;
using CoursesAPI.Services.Exceptions;
using CoursesAPI.Services.Models.Entities;

namespace CoursesAPI.Services.Services
{
	public class CoursesServiceProvider
	{
		private readonly IUnitOfWork _uow;

		private readonly IRepository<CourseInstance> _courseInstances;
		private readonly IRepository<TeacherRegistration> _teacherRegistrations;
		private readonly IRepository<CourseTemplate> _courseTemplates; 
		private readonly IRepository<Person> _persons;

		public CoursesServiceProvider(IUnitOfWork uow)
		{
			_uow = uow;

			_courseInstances      = _uow.GetRepository<CourseInstance>();
			_courseTemplates      = _uow.GetRepository<CourseTemplate>();
			_teacherRegistrations = _uow.GetRepository<TeacherRegistration>();
			_persons              = _uow.GetRepository<Person>();
		}

		/// <summary>
		/// You should implement this function, such that all tests will pass.
        /// Method that adds a teacher to a course with a given id.
        /// The required attributes are given with a view model class.
		/// </summary>
		/// <param name="courseInstanceID">The ID of the course instance which the teacher will be registered to.</param>
		/// <param name="model">The data which indicates which person should be added as a teacher, and in what role.</param>
		/// <returns>Should return basic information about the person.</returns>
		public PersonDTO AddTeacherToCourse(int courseInstanceID, AddTeacherViewModel model)
		{
            var course = _courseInstances.All().SingleOrDefault(x => x.ID == courseInstanceID);
            var teacher = _persons.All().SingleOrDefault(x => x.SSN == model.SSN);
			if(course == null || teacher == null)
            {
                throw new AppObjectNotFoundException();
            }

            var teacherInCourse = _teacherRegistrations.All().SingleOrDefault(x => x.CourseInstanceID == courseInstanceID && x.SSN == model.SSN);
            if(teacherInCourse != null)
            {
                throw new AppValidationException("PERSON_ALREADY_REGISTERED_TEACHER_IN_COURSE");
            }

            var mainTeacher = _teacherRegistrations.All().SingleOrDefault(x => x.CourseInstanceID == courseInstanceID && x.Type == TeacherType.MainTeacher);
            if (mainTeacher != null && model.Type == TeacherType.MainTeacher)
            {
                throw new AppValidationException("COURSE_ALREADY_HAS_A_MAIN_TEACHER");
            }

            TeacherRegistration tr = new TeacherRegistration
            {
                CourseInstanceID = courseInstanceID,
                SSN = teacher.SSN,
                Type = model.Type
            };

            _teacherRegistrations.Add(tr);
            _uow.Save();

            PersonDTO result = new PersonDTO
            {
                Name = teacher.Name,
                SSN = teacher.SSN
            };

			return result;
		}

		/// <summary>
		/// You should write tests for this function. You will also need to
		/// modify it, such that it will correctly return the name of the main
		/// teacher of each course.
		/// </summary>
		/// <param name="semester"></param>
		/// <returns></returns>
		public List<CourseInstanceDTO> GetCourseInstancesBySemester(string semester = null)
		{
			if (string.IsNullOrEmpty(semester))
			{
				semester = "20153";
			}

            var coursesLQ = (from c in _courseInstances.All()
                             join ct in _courseTemplates.All() on c.CourseID equals ct.CourseID
                             where c.SemesterID == semester
                             select new { c, ct });

            var teachersRQ = (from tr in _teacherRegistrations.All()
                              join p in _persons.All() on tr.SSN equals p.SSN
                              where tr.Type == TeacherType.MainTeacher
                              select new { tr, p });

            var courses = (from cLQ in coursesLQ
                            join tRQ in teachersRQ on cLQ.c.ID equals tRQ.tr.CourseInstanceID into ciDTO
                            from ci in ciDTO.DefaultIfEmpty()
                            select new CourseInstanceDTO
                            {
                                Name               = cLQ.ct.Name,
                                TemplateID         = cLQ.ct.CourseID,
                                CourseInstanceID   = cLQ.c.ID,
                                MainTeacher        = (ci == null) ? string.Empty : ci.p.Name
                            }).ToList();

			return courses;
		}
	}
}
