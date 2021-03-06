﻿namespace Collector.Common.RestClient.UnitTests.Client
{
    using System.Threading.Tasks;

    using Collector.Common.RestClient.Exceptions;
    using Collector.Common.RestClient.UnitTests.Fakes;

    using NUnit.Framework;

    using Ploeh.AutoFixture;

    using Rhino.Mocks;

    [TestFixture]
    public class RestApiClient_Test
    {
        private RestApiClient _sut;
        private IRequestHandler _stub;
        private string _context;
        private Fixture _fixture;

        [TestFixtureSetUp]
        protected void TestInitialize()
        {
            _fixture = new Fixture();
            _stub = MockRepository.GenerateMock<IRequestHandler>();
            _stub.Stub(x => x.CallAsync(Arg<RequestWithResponse>.Is.Anything)).Return(Task.FromResult(_fixture.Create<string>()));
            _stub.Stub(x => x.CallAsync(Arg<RequestWithoutResponse>.Is.Anything)).Return(Task.FromResult(_fixture.Create<string>()));

            _context = _fixture.Create<string>();
            _sut = new RestApiClient(_stub, () => _context);
        }

        [Test]
        public void When_executing_call_async_and_the_request_is_not_valid_it_throws_an_exception()
        {
            var request = new RequestWithResponse(new DummyResourceIdentifier());

            Assert.Throws<RestClientCallException>(async () => await _sut.CallAsync(request));
        }

        [Test]
        public void When_executing_call_async_and_the_request_without_response_is_not_valid_it_throws_an_exception()
        {
            var request = new RequestWithoutResponse(new DummyResourceIdentifier()) { StringProperty = null };

            Assert.Throws<RestClientCallException>(async () => await _sut.CallAsync(request));
        }

        [Test]
        public async void When_executing_call_async_and_the_request_does_not_have_a_context_then_a_context_is_added()
        {
            var request = new RequestWithoutResponse(new DummyResourceIdentifier())
            {
                StringProperty = _fixture.Create<string>()
            };

            await _sut.CallAsync(request);

            Assert.AreEqual(_context, request.Context);
        }
    }
}