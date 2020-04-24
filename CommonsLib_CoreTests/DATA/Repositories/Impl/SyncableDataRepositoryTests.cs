using System;
using System.Threading.Tasks;
using CommonsLib_DATA.Attributes;
using CommonsLib_DATA.Repositories;
using CommonsLib_DATA.Repositories.Impl;
using CommonsLib_IOC.Config;
using NUnit.Framework;
using SQLite;

namespace CommonsLib_CoreTests.DATA.Repositories.Impl
{
    [TestFixture]
    public class SyncableDataRepositoryTests: BaseTestClass
    {
        private const string Post1Title1 = "My Test";
        private const string Post1Content1 = "Nice Post content For my test";

        private const string Post2Title1 = "Our Testing";
        private const string Post2Content1 = "Amazing Post content For The Testing.";


        [Test]
        public async Task DataFlow_SyncServerId_Test()
        {
            var lastSuccessfulSyncRepository = IoCManager.Resolver.ResolveInstance<ILastSuccessfulSyncRepository>();
            IPostsRepository postsRepository = new PostsRepository();
            IoCManager.Resolver.InjectProperties(postsRepository);

            var tableName = await postsRepository.FetchTableName();

            var samplePost1 = new PostEntity
            {
                Title = Post1Title1, Content = Post1Content1
            };
            var samplePost2 = new PostEntity
            {
                Title = Post2Title1, Content = Post2Content1
            };

            var savedPosts = await postsRepository.SaveAll(
                new[] {samplePost1, samplePost2}, updateTimestamp: true);

            Assert.IsNotNull(savedPosts);
            Assert.IsNotEmpty(savedPosts);
            Assert.AreEqual(2, savedPosts.Count);

            samplePost1 = savedPosts[0];
            samplePost2 = savedPosts[1];

            Assert.NotNull(samplePost1);
            Assert.NotNull(samplePost2);

            await Task.Delay(1200);
            
            var lastSuccessfulSync = await lastSuccessfulSyncRepository.FindByTableIdAndBackendId(tableName);
            Assert.IsNotNull(lastSuccessfulSync);
            
            var pendingToSync = await postsRepository.FindAllPendingToSync(
                lastSuccessfulSync.SyncTimestamp, new[] {3});
            Assert.IsNotNull(pendingToSync);
            Assert.IsNotEmpty(pendingToSync);
            Assert.AreEqual(2, pendingToSync.Count);


            lastSuccessfulSync.SyncTimestamp = DateTimeOffset.Now;
            lastSuccessfulSync = await lastSuccessfulSyncRepository.Save(lastSuccessfulSync);
            Assert.IsNotNull(lastSuccessfulSync);
            
            pendingToSync = await postsRepository.FindAllPendingToSync(lastSuccessfulSync.SyncTimestamp, 3);
            Assert.IsNotNull(pendingToSync);
            Assert.IsEmpty(pendingToSync);


            samplePost1.Title += " extra.";
            samplePost1 = await postsRepository.Save(samplePost1);
            Assert.IsNotNull(samplePost1);
            
            pendingToSync = await postsRepository.FindAllPendingToSync(lastSuccessfulSync.SyncTimestamp, 3);
            Assert.IsNotNull(pendingToSync);
            Assert.IsNotEmpty(pendingToSync);
            Assert.AreEqual(1, pendingToSync.Count);

        }


        private interface IPostsRepository : ISyncableDataRepository<PostEntity, int> { }
        private class PostsRepository : SyncableDataRepository<PostEntity, int>, IPostsRepository { }
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