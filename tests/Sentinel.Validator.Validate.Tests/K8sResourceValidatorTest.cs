using System.Collections.Generic;
using System.ComponentModel;
using k8s;
using Newtonsoft.Json.Linq;
using Sentinel.Validator.Validate.Models;
using Sentinel.Validator.Validate.Validator;
using Xunit;

namespace Sentinel.Validator.Validate.Tests;

public class K8sResourceValidatorTests
{
    [Fact]
    public void ValidateRules_ShouldReturnEmptyList_WhenValidationModelHasNoValidations()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
                'metadata': {
                    'name': 'test-resource',
                    'namespace': 'test-namespace'
                },
                'kind': 'TestResource'
            }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>()
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOneValidationAndResourceMatches()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
                'metadata': {
                    'name': 'test-resource',
                    'namespace': 'test-namespace'
                },
                'kind': 'TestResource',
                'spec': {
                    'size': 3
                }
            }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
                {
                    new ValidationRuleModel
                    {
                        jsonPath = "$.spec.size",
                        Operator = ">",
                        ExpectedValue = "2"
                    }
                }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Null(result.CapturedString);
            }
        );
    }

    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOneValidationAndResourceDoesNotMatch()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
                'metadata': {
                    'name': 'test-resource',
                    'namespace': 'test-namespace'
                },
                'kind': 'TestResource',
                'spec': {
                    'size': 1
                }
            }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
                {
                    new ValidationRuleModel
                    {
                        jsonPath = "$.spec.size",
                        Operator = ">",
                        ExpectedValue = "2"
                    }
                }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.False(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Null(result.CapturedString);
            }
        );
    }


    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_Equal_Operation_withMatchingResource()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
            'metadata': {
                'name': 'test-resource',
                'namespace': 'test-namespace'
            },
            'kind': 'TestResource',
            'spec': {
                'size': 2
            }
        }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
        {
            new ValidationRuleModel
            {
                jsonPath = "$.spec.size",
                Operator = "==",
                ExpectedValue = "2"
            }
        }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Null(result.CapturedString);
            }
        );
    }


    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_Equal_OperationwithNosign_withMatchingResource()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
            'metadata': {
                'name': 'test-resource',
                'namespace': 'test-namespace'
            },
            'kind': 'TestResource',
            'spec': {
                'size': 2
            }
        }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
        {
            new ValidationRuleModel
            {
                jsonPath = "$.spec.size",
                Operator = "",
                ExpectedValue = "2"
            }
        }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Null(result.CapturedString);
            }
        );
    }

    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_NotEqual_Operation_withMatchingResource()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
            'metadata': {
                'name': 'test-resource',
                'namespace': 'test-namespace'
            },
            'kind': 'TestResource',
            'spec': {
                'size': 2
            }
        }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
            {
                new ValidationRuleModel
                {
                    jsonPath = "$.spec.size",
                    Operator = "!=",
                    ExpectedValue = "3"
                }
            }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Null(result.CapturedString);
            }
        );
    }



    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_NotNull_Operation_withMatchingResource()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
            'metadata': {
                'name': 'test-resource',
                'namespace': 'test-namespace'
            },
            'kind': 'TestResource',
            'spec': {
                'size': 2
            }
        }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
            {
                new ValidationRuleModel
                {
                    jsonPath = "$.spec.size",
                    Operator = "!null"
                }
            }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Null(result.CapturedString);
            }
        );
    }


    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_Null_Operation_withMatchingResource()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
            'metadata': {
                'name': 'test-resource',
                'namespace': 'test-namespace'
            },
            'kind': 'TestResource',
            'spec': {
                'size': null
            }
        }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
            {
                new ValidationRuleModel
                {
                    jsonPath = "$.spec.size",
                    Operator = "null"
                }
            }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Null(result.CapturedString);
            }
        );
    }


    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_Lessthan_Operation_withMatchingResource()
    {
        // Arrange
        var eventType = k8s.WatchEventType.Added;
        var resource = JToken.Parse(@"{
            'metadata': {
                'name': 'test-resource',
                'namespace': 'test-namespace'
            },
            'kind': 'TestResource',
            'spec': {
                'size': 5
            }
        }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
            {
                new ValidationRuleModel
                {
                    jsonPath = "$.spec.size",
                    CaptureJsonPath= "$.spec.size",
                    Operator = "<",
                    ExpectedValue = 7
                }
            }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Equal("5", result.CapturedString);
            }
        );
    }



    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_LessthanorEqual_Operation_withMatchingResource()
    {
        // Arrange
        var eventType = WatchEventType.Added;
        var resource = JToken.Parse(@"{
                    'metadata': {
                        'name': 'test-resource',
                        'namespace': 'test-namespace'
                    },
                    'kind': 'TestResource',
                    'spec': {
                        'size': 5
                    }
                }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
                    {
                        new ValidationRuleModel
                        {
                            jsonPath = "$.spec.size",
                            CaptureJsonPath= "$.spec.size",
                            Operator = "<=",
                            ExpectedValue = 5
                        }
                    }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Equal("5", result.CapturedString);
            }
        );
    }




    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_GreaterthanorEqual_Operation_withMatchingResource()
    {
        // Arrange
        var eventType = WatchEventType.Added;
        var resource = JToken.Parse(@"{
                    'metadata': {
                        'name': 'test-resource',
                        'namespace': 'test-namespace'
                    },
                    'kind': 'TestResource',
                    'spec': {
                        'size': 5
                    }
                }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
                    {
                        new ValidationRuleModel
                        {
                            jsonPath = "$.spec.size",
                            CaptureJsonPath= "$.spec.size",
                            Operator = ">=",
                            ExpectedValue = 7
                        }
                    }
        };

        // Act
        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Equal("5", result.CapturedString);
            }
        );
    }


    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_Contains_Operation_withMatchingResource()
    {
        var eventType = WatchEventType.Added;
        var resource = JToken.Parse(@"{
                    'metadata': {
                        'name': 'test-resource',
                        'namespace': 'test-namespace',
                        'labels': {
                            'app': 'test-app'
                        }
                    },
                    'kind': 'TestResource',
                    'spec': {
                        'size': 5
                    }
                }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
                    {
                        new ValidationRuleModel
                        {
                            jsonPath = "$.metadata.labels.app",
                            CaptureJsonPath= "$.metadata.labels.app",
                            Operator = "contains",
                            ExpectedValue = "app"
                        }
                    }
        };


        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Equal("test-app", result.CapturedString);
            }
        );
    }


    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WhenValidationModelHasOne_NotContains_Operation_withMatchingResource()
    {
        var eventType = WatchEventType.Added;
        var resource = JToken.Parse(@"{
                    'metadata': {
                        'name': 'test-resource',
                        'namespace': 'test-namespace',
                        'labels': {
                            'app': 'test-app'
                        }
                    },
                    'kind': 'TestResource',
                    'spec': {
                        'size': 5
                    }
                }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
                    {
                        new ValidationRuleModel
                        {
                            jsonPath = "$.metadata.labels.app",
                            CaptureJsonPath= "$.metadata.labels.app",
                            Operator = "!contains",
                            ExpectedValue = "prod"
                        }
                    }
        };


        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Equal("test-app", result.CapturedString);
            }
        );
    }


    [Fact]
    public void ValidateRules_ShouldReturnOneResult_WithoutaCapture_when_CaptureJsonPath_defined_but_no_match()
    {
        var eventType = WatchEventType.Added;
        var resource = JToken.Parse(@"{
                    'metadata': {
                        'name': 'test-resource',
                        'namespace': 'test-namespace',
                        'labels': {
                            'app': 'test-app'
                        }
                    },
                    'kind': 'TestResource',
                    'spec': {
                        'size': 5
                    }
                }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
                    {
                        new ValidationRuleModel
                        {
                            jsonPath = "$.metadata.labels.app",
                            CaptureJsonPath= "$.metadata.labels.helm",
                            Operator = "!contains",
                            ExpectedValue = "prod"
                        }
                    }
        };


        var results = K8sResourceValidator.ValidateRules(eventType, resource, validationModel);

        // Assert
        Assert.Collection(results,
            result =>
            {
                Assert.Equal(validationModel.Validations[0], result.Rule);
                Assert.True(result.Isvalid);
                Assert.Equal("test-resource", result.ResourceName);
                Assert.Equal("TestResource", result.ResourceType);
                Assert.Equal("test-namespace", result.ResourceNamespace);
                Assert.Equal(null, result.CapturedString);
            }
        );
    }


    [Fact]
    public void ValidateRules_ShouldThrowExceptionWhen_ValidationRuleModel_Operator_doesnt_match_with_avaiable_operations()
    {
        var eventType = WatchEventType.Added;
        var resource = JToken.Parse(@"{
                    'metadata': {
                       // 'name': 'test-resource',
                        //'namespace': 'test-namespace',
                        'labels': {
                            'app': 'test-app'
                        }
                    },
                    'kind': 'TestResource',
                    'spec': {
                        'size': 5
                    }
                }");
        var validationModel = new ValidationModel
        {
            Validations = new List<ValidationRuleModel>
                    {
                        new ValidationRuleModel
                        {
                            jsonPath = "$.metadata.labels.app",
                            Operator = "invalid_operator",
                            ExpectedValue = "prod"
                        }
                    }
        };

        // Assert
        Assert.Throws<ArgumentException>(() => K8sResourceValidator.ValidateRules(eventType, resource, validationModel));
    }

}
