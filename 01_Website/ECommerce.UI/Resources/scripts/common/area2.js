(function () {

    function getData(ele, evt) {
        evt.stopPropagation();
        var $this = $(ele),
            $select_area_a = $('.select_area dt>a:first'),
            rel = $this.attr('rel'),
            psysno = $this.attr('psysno'),
            loaded = $this.attr('loaded');
        if (loaded == "0") {
            if (rel == 1) {
                setName('', 1);
                setName('', 2);
                $select_area_a.text($select_area_a.text().replace(/\/+$/, ''));
                $city.val('');
                $district.val('');
                $('.select_area .tabc:eq(2) .selectareap').html('');
                $('.select_area .tab>a:eq(2)').attr({'loaded': '0', 'psysno':'0', 'sysno':'0'});
                getCities(psysno);
            } else if (rel = 2) {
                setName('', 2);
                $select_area_a.text($select_area_a.text().replace(/\/+$/, ''));
                $district.val('');
                getDistricts(psysno);
            }
        }
    }

    var $province = null,
        $city = null,
        $district = null;

    function clearData() {
        $province.val('');
        $city.val('');
        $district.val('');
        $('.select_area dt>a:first').addClass('gray').text('请选择省市区');
        $('.select_area .tab a').attr('loaded', '0');
        $('.select_area .tab a:first').click();
        $('.select_area .tabc a').removeClass('now');
    }

    function getCities(provinceSysno, selCitySysno) {
        if (provinceSysno > 0) {
            $.ajax({
                type: "post",
                url: "/Home/GetAllCity",
                dataType: "json",
                data: { proviceSysNo: provinceSysno },
                beforeSend: function (XMLHttpRequest) {
                    $('.select_area .tabc:eq(1) .selectareap').html('<div class="loading">数据加载中...</div>');
                },
                success: function (data) {
                    var html = '<div class="cls">', name = '';
                    for (i = 0; i < data.length; i++) {
                        if (selCitySysno === data[i].SysNo) name = data[i].CityName;
                        html += ' <a href="javascript:void(0)" value="' + data[i].SysNo + '"' + (selCitySysno === data[i].SysNo ? ' class="now"' : '') + '>' + data[i].CityName + '</a>';
                    };
                    html += '</div>';
                    $('.select_area .tabc:eq(1) .selectareap').html(html);
                    $('.select_area .tab>a:eq(1)').attr('loaded', '1');
                    if (name.length > 0) setName(name, 1);
                }
            });
        }
    }

    function getDistricts(citySysno, selDisSysno) {
        if (citySysno > 0) {
            $.ajax({
                type: "post",
                url: "/Home/GetAllDistrict",
                dataType: "json",
                data: { citySysNo: citySysno },
                beforeSend: function (XMLHttpRequest) {
                    $('.select_area .tabc:eq(2) .selectareap').html('<div class="loading">数据加载中...</div>');
                },
                success: function (data) {
                    var html = '<div class="cls">', name = '';
                    for (i = 0; i < data.length; i++) {
                        if (selDisSysno === data[i].SysNo) name = data[i].DistrictName;
                        html += ' <a href="javascript:void(0)" value="' + data[i].SysNo + '"' + (selDisSysno === data[i].SysNo ? ' class="now"' : '') + '>' + data[i].DistrictName + '</a>';
                    };
                    html += '</div>';
                    $('.select_area .tabc:eq(2) .selectareap').html(html);
                    $('.select_area .tab>a:eq(2)').attr('loaded', '1');
                    if (name.length > 0) setName(name, 2);
                }
            });
        }
    }

    function setName(name, index) {
        var names = $('.select_area dt>a:first').removeClass('gray').text().split('/');
        if (names.length < index + 1) {
            for (var i = 0; i < index + 1 - names.length; i++) {
                names.push('');
            }
        }
        names[index] = name;
        $('.select_area dt>a:first').text(names.join('/'));
    }

    function switchToTabAndBindData($tab,evt) {
        var $parent = $tab.parents();
        $parent.find('.tab a').removeClass('now').end().find('.tabc').hide();
        $parent.find('.tabc:eq(' + $tab.attr('rel') + ')').show();
        $tab.addClass('now');
        getData($tab.get(0), evt);
    }

    function init() {

        $province = $('.select_area input[name="Province"]');
        $city = $('.select_area input[name="City"]');
        $district = $('.select_area input[name="District"]');

        if (tabPanel && typeof tabPanel === "function") {
            tabPanel($(".tab"));
        }
        $('.select_area dt').click(function (evt) {
            evt.stopPropagation();
            var $this = $(this),
                $select_area = $this.parents('.select_area');
            if ($select_area.is('.select_expand')) {
                $select_area.removeClass('select_expand').find('dd').hide();
            } else {
                $select_area.find('>dd>tab>a').removeClass('now').end().find('>dd>tab>a:first').addClass('now');
                $select_area.addClass('select_expand').find('>dd').show();
                $(document).one('click', function () {
                    if ($select_area.is('.select_expand')) {
                        $select_area.removeClass('select_expand').find('dd').hide();
                    }
                });
            }
        });

        $('.selectareap').delegate('a', 'click', function (evt) {
            evt.stopPropagation();
            var $this = $(this),
                $select_area = $this.parents('.select_area'),
                index = $this.parents('.tabc').index(),
            $tab = $this.parents('.tabc').siblings('.tab').find(':eq(' + index + ')'),
            $thistab = $this.parents('.tabc').siblings('.tab').find(':eq(' + (index-1) + ')');

            setName($this.text(), index - 1);

            $this.parents('.tabc').find('a').removeClass('now');
            $this.addClass('now');
            if (parseInt($tab.attr('psysno')) == parseInt($this.attr('value'))) {
                $tab.attr('loaded', '1');
            } else {
                $tab.attr('loaded', '0');
            }
            $thistab.attr('sysno', $this.attr('value'));
            $tab.attr('psysno', $this.attr('value'));
            if (index == 1) {
                $thistab.nextAll().attr('sysno', '0');
                $select_area.find('input[name="Province"]').val($this.attr('value'));
                switchToTabAndBindData($tab, evt);
            } else if (index == 2) {
                $thistab.nextAll().attr('sysno', '0');
                $select_area.find('input[name="City"]').val($this.attr('value'));
                switchToTabAndBindData($tab, evt);
            } else {
                $select_area.removeClass('select_expand').find('input[name="District"]')
                 .val($this.attr('value')).end().find('>dd').hide();
            }
        })

        var selProvinceSysno = parseInt($('.select_area input[name="Province"]').val()),
            selCitySysno = parseInt($('.select_area input[name="City"]').val())
        selDistrictSysno = parseInt($('.select_area input[name="District"]').val());

        if (selProvinceSysno > 0) {
            var selProvince = $('.select_area .tabc:first a[value="' + selProvinceSysno + '"]');
            selProvince.addClass('now');
            setName(selProvince.text(), 0);
            getCities(selProvinceSysno, selCitySysno);
        }
        if (selCitySysno > 0) {
            getDistricts(selCitySysno, selDistrictSysno);
        }
    }

    usingNamespace("Biz.Common")["Area2"] = {
        getData: getData,
        clearData: clearData,
        InitComponent: init
    };
})();