using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using SmartHome.Application.Exceptions;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Application.Services;
using SmartHome.Dto.Area;
using SmartHome.Dto.Controller;
using SmartHome.Enum;
using SmartHome.Domain.Entities;
using Xunit;

namespace SmartHome.Tests.Unit.Application.Services
{
    public class ControllerServiceTests
    {
        private readonly Mock<IControllerRepository> _controllerRepositoryMock;
        private readonly Mock<IValidator<CreateControllerDto>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateControllerDto>> _updateValidatorMock;
        private readonly Mock<IValidator<DeleteControllerDto>> _deleteValidatorMock;
        private readonly Mock<IValidator<GetControllerDto>> _getValidatorMock;
        private readonly Mock<IMapper> _mapperMock;

        public ControllerServiceTests()
        {
            _controllerRepositoryMock = new Mock<IControllerRepository>();
            _createValidatorMock = new Mock<IValidator<CreateControllerDto>>();
            _updateValidatorMock = new Mock<IValidator<UpdateControllerDto>>();
            _deleteValidatorMock = new Mock<IValidator<DeleteControllerDto>>();
            _getValidatorMock = new Mock<IValidator<GetControllerDto>>();
            _mapperMock = new Mock<IMapper>();
        }

        private ControllerService CreateService()
        {
            return new ControllerService(
                _controllerRepositoryMock.Object,
                _mapperMock.Object,
                _createValidatorMock.Object,
                _updateValidatorMock.Object,
                _deleteValidatorMock.Object,
                _getValidatorMock.Object
            );
        }

        #region CreateController Tests

        [Fact]
        public async Task CreateController_ShouldCallRepository_WhenDtoIsValid()
        {
            // Arrange
            var service = CreateService();

            var createDto = new CreateControllerDto { Name = "Controller1" };
            // Setup validation as successful.
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Setup mapping from DTO to domain entity.
            var controllerEntity = new Controller();
            _mapperMock.Setup(m => m.Map<Controller>(createDto))
                .Returns(controllerEntity);

            // Act
            await service.CreateController(createDto);

            // Assert: Verify that repository CreateController is called.
            _controllerRepositoryMock.Verify(r => r.CreateController(It.Is<Controller>(c =>
                c == controllerEntity &&
                c.Id != Guid.Empty &&    // New Guid assigned
                c.LastSeen <= DateTime.UtcNow)), Times.Once);
        }

        [Fact]
        public async Task CreateController_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var service = CreateService();

            var createDto = new CreateControllerDto { Name = "Controller1" };
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            };
            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<FluentValidationException>(() => service.CreateController(createDto));
            Assert.Equal(ApiResponseStatus.Error.ToString(), ex.Status);
            Assert.Equal("Bad Request Body", ex.Message);
            Assert.Equal(errors, ex.Data);
        }

        [Fact]
        public async Task CreateController_ShouldThrowNullReferenceException_WhenDtoIsNull()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.CreateController(null));
        }

        #endregion

        #region DeleteController Tests

        [Fact]
        public async Task DeleteController_ShouldCallRepository_WhenControllerExistsAndDtoValid()
        {
            // Arrange
            var service = CreateService();

            var deleteDto = new DeleteControllerDto { Id = Guid.NewGuid() };
            _deleteValidatorMock.Setup(v => v.ValidateAsync(deleteDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Simulate repository returning an existing controller.
            var controllerEntity = new Controller { Id = deleteDto.Id };
            _controllerRepositoryMock.Setup(r => r.GetController(deleteDto.Id))
                .ReturnsAsync(controllerEntity);

            // Act
            await service.DeleteController(deleteDto);

            // Assert: Verify that DeleteController is called.
            _controllerRepositoryMock.Verify(r => r.DeleteController(controllerEntity), Times.Once);
        }

        [Fact]
        public async Task DeleteController_ShouldNotCallDelete_WhenControllerDoesNotExist()
        {
            // Arrange
            var service = CreateService();

            var deleteDto = new DeleteControllerDto { Id = Guid.NewGuid() };
            _deleteValidatorMock.Setup(v => v.ValidateAsync(deleteDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Simulate repository returning null.
            _controllerRepositoryMock.Setup(r => r.GetController(deleteDto.Id))
                .ReturnsAsync((Controller)null);

            // Act
            await service.DeleteController(deleteDto);

            // Assert: Verify that DeleteController is never called.
            _controllerRepositoryMock.Verify(r => r.DeleteController(It.IsAny<Controller>()), Times.Never);
        }

        [Fact]
        public async Task DeleteController_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var service = CreateService();

            var deleteDto = new DeleteControllerDto { Id = Guid.NewGuid() };
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Id", "Invalid Id")
            };
            _deleteValidatorMock.Setup(v => v.ValidateAsync(deleteDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<FluentValidationException>(() => service.DeleteController(deleteDto));
            Assert.Equal(ApiResponseStatus.Error.ToString(), ex.Status);
        }

        [Fact]
        public async Task DeleteController_ShouldThrowNullReferenceException_WhenDtoIsNull()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.DeleteController(null));
        }

        #endregion

        #region GetController Tests

        [Fact]
        public async Task GetController_ShouldReturnMappedDto_WhenControllerExistsAndDtoValid()
        {
            // Arrange
            var service = CreateService();

            var getDto = new GetControllerDto { Id = Guid.NewGuid() };
            _getValidatorMock.Setup(v => v.ValidateAsync(getDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            // Simulate repository returning a controller.
            var controllerEntity = new Controller { Id = getDto.Id, IPAddress = "192.168.1.1" };
            _controllerRepositoryMock.Setup(r => r.GetController(getDto.Id))
                .ReturnsAsync(controllerEntity);

            var expectedDto = new ControllerDto { Id = getDto.Id, IPAddress = "192.168.1.1" };
            _mapperMock.Setup(m => m.Map<ControllerDto>(controllerEntity))
                .Returns(expectedDto);

            // Act
            var result = await service.GetController(getDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.IPAddress, result.IPAddress);
        }

        [Fact]
        public async Task GetController_ShouldReturnNull_WhenControllerDoesNotExist()
        {
            // Arrange
            var service = CreateService();

            var getDto = new GetControllerDto { Id = Guid.NewGuid() };
            _getValidatorMock.Setup(v => v.ValidateAsync(getDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _controllerRepositoryMock.Setup(r => r.GetController(getDto.Id))
                .ReturnsAsync((Controller)null);

            // Act
            var result = await service.GetController(getDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetController_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var service = CreateService();

            var getDto = new GetControllerDto { Id = Guid.NewGuid() };
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Id", "Invalid Id")
            };
            _getValidatorMock.Setup(v => v.ValidateAsync(getDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<FluentValidationException>(() => service.GetController(getDto));
            Assert.Equal(ApiResponseStatus.Error.ToString(), ex.Status);
        }

        [Fact]
        public async Task GetController_ShouldThrowNullReferenceException_WhenDtoIsNull()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.GetController(null));
        }

        #endregion

        #region GetControllerIpAsync Tests

        [Fact]
        public async Task GetControllerIpAsync_ShouldReturnIp_WhenControllerExists()
        {
            // Arrange
            var service = CreateService();

            var id = Guid.NewGuid();
            var controllerEntity = new Controller { Id = id, IPAddress = "10.0.0.1" };
            _controllerRepositoryMock.Setup(r => r.GetController(id))
                .ReturnsAsync(controllerEntity);

            // Act
            var ip = await service.GetControllerIpAsync(id);

            // Assert
            Assert.Equal("10.0.0.1", ip);
        }

        [Fact]
        public async Task GetControllerIpAsync_ShouldThrowKeyNotFoundException_WhenControllerDoesNotExist()
        {
            // Arrange
            var service = CreateService();

            var id = Guid.NewGuid();
            _controllerRepositoryMock.Setup(r => r.GetController(id))
                .ReturnsAsync((Controller)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetControllerIpAsync(id));
        }

        #endregion

        #region GetControllers Tests

        [Fact]
        public async Task GetControllers_ShouldReturnMappedList()
        {
            // Arrange
            var service = CreateService();

            var controllers = new List<Controller>
            {
                new Controller { Id = Guid.NewGuid(), IPAddress = "10.0.0.1" },
                new Controller { Id = Guid.NewGuid(), IPAddress = "10.0.0.2" }
            };

            _controllerRepositoryMock.Setup(r => r.GetControllers())
                .ReturnsAsync(controllers);

            var mappedList = new List<ControllerDto>
            {
                new ControllerDto { Id = controllers[0].Id, IPAddress = "10.0.0.1" },
                new ControllerDto { Id = controllers[1].Id, IPAddress = "10.0.0.2" }
            };
            _mapperMock.Setup(m => m.Map<List<ControllerDto>>(controllers))
                .Returns(mappedList);

            // Act
            var result = await service.GetControllers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("10.0.0.1", result[0].IPAddress);
            Assert.Equal("10.0.0.2", result[1].IPAddress);
        }

        #endregion

        #region UpdateController Tests

        [Fact]
        public async Task UpdateController_ShouldCallRepository_WhenDtoIsValidAndControllerExists()
        {
            // Arrange
            var service = CreateService();

            var updateDto = new UpdateControllerDto { Id = Guid.NewGuid(), Name = "UpdatedName" };
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var controllerEntity = new Controller { Id = updateDto.Id, Name = "OldName" };
            _controllerRepositoryMock.Setup(r => r.GetController(updateDto.Id))
                .ReturnsAsync(controllerEntity);

            // Setup mapper to map updateDto onto controllerEntity.
            _mapperMock.Setup(m => m.Map(updateDto, controllerEntity))
                .Callback<UpdateControllerDto, Controller>((src, dest) => dest.Name = src.Name);

            // Act
            await service.UpdateController(updateDto);

            // Assert: Verify that UpdateController is called.
            _controllerRepositoryMock.Verify(r => r.UpdateController(It.Is<Controller>(c =>
                c.Id == updateDto.Id && c.Name == "UpdatedName" &&
                c.LastSeen <= DateTime.UtcNow)), Times.Once);
        }

        [Fact]
        public async Task UpdateController_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var service = CreateService();

            var updateDto = new UpdateControllerDto { Id = Guid.NewGuid(), Name = "UpdatedName" };
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Invalid Name")
            };
            _updateValidatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(errors));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<FluentValidationException>(() => service.UpdateController(updateDto));
            Assert.Equal(ApiResponseStatus.Error.ToString(), ex.Status);
        }

        [Fact]
        public async Task UpdateController_ShouldThrowNullReferenceException_WhenDtoIsNull()
        {
            // Arrange
            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.UpdateController(null));
        }

        #endregion
    }
}
