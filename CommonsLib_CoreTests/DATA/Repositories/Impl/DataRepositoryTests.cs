using System;
using System.Threading.Tasks;
using CommonsLib_DAL.Data.Impl;
using CommonsLib_DAL.Errors;
using CommonsLib_DATA.Attributes;
using CommonsLib_DATA.Repositories;
using CommonsLib_DATA.Repositories.Impl;
using CommonsLib_IOC.Config;
using NUnit.Framework;
using SQLite;

namespace CommonsLib_CoreTests.DATA.Repositories.Impl
{
    [TestFixture]
    public class DataRepositoryTests: BaseTestClass
    {
        private const string Post1Title1 = "My Test";
        private const string Post1Content1 = "Nice Post content For my test";
        private const string Post1Content2 = "Great Post content for the test.";

        private const string Post2Title1 = "Our Testing";
        private const string Post2Content1 = "Amazing Post content For The Testing.";
        
        
        [Test]
        public async Task FetchTableName_Success()
        {
            IPostsRepository postsRepository = new PostsRepository();
            IoCManager.Resolver.InjectProperties(postsRepository);

            var tableName = await postsRepository.FetchTableName();
            
            Assert.IsNotNull(tableName);
            Assert.AreEqual("post", tableName);
        }

        [Test]
        public async Task FetchColumnNameFromAttribute_Success()
        {
            IPostsRepository postsRepository = new PostsRepository();
            IoCManager.Resolver.InjectProperties(postsRepository);

            var pkCol = await postsRepository.FetchColumnNameFromAttribute<PrimaryKeyAttribute>();
            Assert.IsNotNull(pkCol);
            Assert.IsNotEmpty(pkCol);
            Assert.AreEqual("post_id", pkCol);
            
            var createdAtCol = await postsRepository.FetchColumnNameFromAttribute<CreatedAtColumnAttribute>();
            Assert.IsNotNull(createdAtCol);
            Assert.IsNotEmpty(createdAtCol);
            Assert.AreEqual("created_at", createdAtCol);
            
            var updatedAtCol = await postsRepository.FetchColumnNameFromAttribute<UpdatedAtColumnAttribute>();
            Assert.IsNotNull(updatedAtCol);
            Assert.IsNotEmpty(updatedAtCol);
            Assert.AreEqual("updated_at", updatedAtCol);

            var uniqueCol = await postsRepository.FetchColumnNameFromAttribute<UniqueAttribute>();
            Assert.IsNotNull(uniqueCol);
            Assert.IsEmpty(uniqueCol);
        }
        
        
        [Test, Repeat(2)]
        public async Task DataFlow_InsertPostEntity_Test()
        {
            IPostsRepository postsRepository = new PostsRepository();
            IoCManager.Resolver.InjectProperties(postsRepository);
            
            var findAllResult = await postsRepository.FindAll();
            Assert.IsEmpty(findAllResult);
            
            var samplePost1 = new PostEntity
            {
                Title = Post1Title1, Content = Post1Content1
            };
            Assert.AreEqual(0, samplePost1.PostId);
            
            samplePost1 = await postsRepository.Save(samplePost1);
            Assert.AreNotEqual(0, samplePost1.PostId);

            findAllResult = await postsRepository.FindAll();
            Assert.IsNotEmpty(findAllResult);
            Assert.AreEqual(1, findAllResult.Count);

            var postFound = await postsRepository.FindById(1);
            Assert.IsNotNull(postFound);
            Assert.IsNotNull(postFound.PostId);
            Assert.AreEqual(1, postFound.PostId);
            Assert.AreEqual(Post1Title1, postFound.Title);
            Assert.AreEqual(Post1Content1, postFound.Content);

            Assert.ThrowsAsync<GrException>(() => postsRepository.FindById(1000));
        }


        [Test]
        public async Task DataFlow_InsertUpdatePost_Test()
        {
            IPostsRepository postsRepository = new PostsRepository();
            IoCManager.Resolver.InjectProperties(postsRepository);

            var findAllResult = await postsRepository.FindAll();
            Assert.IsEmpty(findAllResult);
            
            var samplePost1 = new PostEntity
            {
                Title = Post1Title1, Content = Post1Content1
            };
            var samplePost2 = new PostEntity
            {
                Title = Post2Title1, Content = Post2Content1
            };

            var savedPosts = await postsRepository.SaveAll(
                new[] {samplePost1, samplePost2});

            Assert.IsNotNull(savedPosts);
            Assert.IsNotEmpty(savedPosts);
            Assert.AreEqual(2, savedPosts.Count);

            samplePost1 = savedPosts[0];
            samplePost2 = savedPosts[1];
            
            Assert.AreNotEqual(0, samplePost1.PostId);
            Assert.AreEqual(1, samplePost1.PostId);
            Assert.AreEqual(Post1Title1, samplePost1.Title);
            Assert.AreEqual(Post1Content1, samplePost1.Content);
            
            Assert.AreNotEqual(0, samplePost2.PostId);
            Assert.AreEqual(2, samplePost2.PostId);
            Assert.AreEqual(Post2Title1, samplePost2.Title);
            Assert.AreEqual(Post2Content1, samplePost2.Content);

            
            samplePost1.Content = Post1Content2;
            samplePost1 = await postsRepository.Save(samplePost1);
            Assert.AreEqual(samplePost1.Content, Post1Content2);
            
            findAllResult = await postsRepository.FindAll();
            Assert.IsNotEmpty(findAllResult);
            Assert.AreEqual(2, findAllResult.Count);

            var post1Found = await postsRepository.FindById(1);
            Assert.IsNotNull(post1Found);
            Assert.AreEqual(1, post1Found.PostId);
            Assert.AreEqual(Post1Title1, post1Found.Title);
            Assert.AreEqual(Post1Content2, post1Found.Content);

            var post2Found = await postsRepository.FindById(2);
            Assert.IsNotNull(post2Found);
            Assert.AreEqual(2, post2Found.PostId);
            Assert.AreEqual(Post2Title1, post2Found.Title);
            Assert.AreEqual(Post2Content1, post2Found.Content);

            var allPostsFound = await postsRepository.FindAllById(new[] {1, 2, 1000});
            Assert.IsNotNull(allPostsFound);
            Assert.AreEqual(2, allPostsFound.Count);

        }


        [Test]
        public async Task DataFlow_FindPageAndMap_Test()
        {
            IPostsRepository postsRepository = new PostsRepository();
            IoCManager.Resolver.InjectProperties(postsRepository);
            
            var findAllResult = await postsRepository.FindAll();
            Assert.IsEmpty(findAllResult);

            var postCount = 0;
            var savedPosts = await postsRepository.SaveAll(
                new[]
                {
                    new PostEntity { Title = $"{postCount}", Content = $"{postCount++}"},
                    new PostEntity { Title = $"{postCount}", Content = $"{postCount++}"},
                    new PostEntity { Title = $"{postCount}", Content = $"{postCount++}"},
                    new PostEntity { Title = $"{postCount}", Content = $"{postCount++}"},
                    new PostEntity { Title = $"{postCount}", Content = $"{postCount++}"},
                });
            
            Assert.IsNotNull(savedPosts);
            Assert.IsNotEmpty(savedPosts);
            Assert.AreEqual(postCount, savedPosts.Count);

            var foundPage = await postsRepository.FindAll(Pageable.Of(1, 2));
            
            Assert.IsNotNull(foundPage);
            Assert.IsNotNull(foundPage.PageInfo);
            Assert.AreEqual(postCount, foundPage.Total);
            
            Assert.IsNotNull(foundPage.Content);
            Assert.AreEqual(2, foundPage.Content.Count);
            
            Assert.IsNotNull(foundPage.Content[0]);
            Assert.AreEqual("2", foundPage.Content[0].Title);
            
            Assert.IsNotNull(foundPage.Content[1]);
            Assert.AreEqual("3", foundPage.Content[1].Title);

            
            var mappedPage = foundPage.Map(entity => entity.Title);
            
            Assert.IsNotNull(mappedPage);
            Assert.IsNotNull(mappedPage.PageInfo);
            Assert.AreEqual(postCount, mappedPage.Total);
            
            Assert.IsNotNull(mappedPage.Content);
            Assert.AreEqual(2, mappedPage.Content.Count);
            Assert.AreEqual("2", mappedPage.Content[0]);
            Assert.AreEqual("3", mappedPage.Content[1]);

        }


        private interface IPostsRepository : IDataRepository<PostEntity, int> { }
        private class PostsRepository : DataRepository<PostEntity, int>, IPostsRepository { }
        [Table("post")]
        private class PostEntity
        {
            [Column("post_id"), PrimaryKey, AutoIncrement]
            public int PostId { get; set; }

            [Column("title"), NotNull]
            public string Title { get; set; }
            [Column("content"), NotNull]
            public string Content { get; set; }

            [IsDeletedFlagColumn(IsDeletedValue = true)]
            [Column("is_deleted"), NotNull]
            public bool IsDeleted { get; set; }

            [CreatedAtColumn]
            [Column("created_at"), NotNull]
            public DateTimeOffset CreatedAt { get; set; }

            [UpdatedAtColumn]
            [Column("updated_at"), NotNull]
            public DateTimeOffset UpdatedAt { get; set; }
        }
    }

}