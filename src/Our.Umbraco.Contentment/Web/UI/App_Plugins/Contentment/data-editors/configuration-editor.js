﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

angular.module("umbraco").controller("Our.Umbraco.Contentment.DataEditors.ConfigurationEditor.Controller", [
    "$scope",
    "editorService",
    function ($scope, editorService) {

        // console.log("config-editor.model", $scope.model);

        var defaultConfig = {
            items: [],
            maxItems: 0,
            disableSorting: 0,
            allowEdit: 1,
            allowRemove: 1,
            enableFilter: 0,
            orderBy: "name",
            overlaySize: "large"
        };
        var config = angular.extend({}, defaultConfig, $scope.model.config);

        var vm = this;

        function init() {

            $scope.model.value = $scope.model.value || [];

            if (_.isArray($scope.model.value) === false) {
                $scope.model.value = [$scope.model.value];
            }

            // TODO: [LK:2019-06-06] If there are no available items, then show a messaging saying so. [LK]

            vm.allowAdd = (config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems;
            vm.allowEdit = Object.toBoolean(config.allowEdit);
            vm.allowRemove = Object.toBoolean(config.allowRemove);
            vm.published = true;
            vm.sortable = Object.toBoolean(config.disableSorting) === false && (config.maxItems !== 1 && config.maxItems !== "1");

            vm.sortableOptions = {
                axis: "y",
                containment: "parent",
                cursor: "move",
                disabled: vm.sortable === false,
                opacity: 0.7,
                scroll: true,
                tolerance: "pointer",
                stop: function (e, ui) {
                    setDirty();
                }
            };

            vm.add = add;
            vm.edit = edit;
            vm.remove = remove;
        };

        function add($event) {
            var configPicker = {
                view: "/App_Plugins/Contentment/data-editors/configuration-editor.overlay.html",
                size: "small",
                config: {
                    mode: "select",
                    items: angular.copy(config.items),
                    enableFilter: Object.toBoolean(config.enableFilter),
                    overlaySize: config.overlaySize,
                    orderBy: config.orderBy,
                },
                value: {},
                submit: function (model) {

                    $scope.model.value.push(model);

                    if ((config.maxItems !== 0 && config.maxItems !== "0") && $scope.model.value.length >= config.maxItems) {
                        vm.allowAdd = false;
                    }

                    setDirty();

                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };

            editorService.open(configPicker);
        };

        function edit($index, item) {

            var editor = _.find(config.items, function (x) {
                return x.type === item.type;
            });

            if (!editor) {
                // TODO: What to do if we don't find the config? [LK]
                console.log("Unable to find error:", item.type)
            }

            var configPicker = {
                view: "/App_Plugins/Contentment/data-editors/configuration-editor.overlay.html",
                size: config.overlaySize,
                config: {
                    mode: "edit",
                    editor: editor,
                },
                value: item,
                submit: function (model) {
                    $scope.model.value[$index] = model;
                    setDirty();
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };

            editorService.open(configPicker);
        };

        function remove($index) {
            $scope.model.value.splice($index, 1);

            if ((config.maxItems === 0 || config.maxItems === "0") || $scope.model.value.length < config.maxItems) {
                vm.allowAdd = true;
            }

            setDirty();
        };

        function setDirty() {
            if ($scope.propertyForm) {
                $scope.propertyForm.$setDirty();
            }
        };

        init();
    }
]);
