Ext.onReady(function() {
    Ext.manifest = {
        widgets: [
            {
                xtype:  'widget.button',
                ui:     'snapshot-add-btn-small'
            },
            {
                xtype:  'widget.button',
                ui:     'snapshot-add-btn-small'
            },
			{
                xtype:  'widget.button',
                ui:     'snapshot-navigation-btn-small'
            },
			{
                xtype:  'widget.button',
                ui:     'snapshot-sidebar-btn-small'
            },
			{
                xtype:  'widget.button',
                ui:     'snapshot-submenu-btn-small'
            },
			{
                xtype:  'widget.button',
                ui:     'snapshot-cancel-btn-small'
            },
			{
                xtype:  'widget.button',
                ui:     'snapshot-pagination-btn-small'
            },
			{
                xtype:  'widget.panel',
                ui:     'snapshot-grid-widget-panel'
            },
            {
                xtype:  'widget.toolbar',
                ui:     'snapshot-pagination-toolbar'
            }
        ]
    };
});