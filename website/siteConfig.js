/**
 * Copyright (c) 2017-present, Facebook, Inc.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

const siteConfig = {
  title: 'Map generation' /* title for your website */,
  tagline: 'An application for generating map layouts for games',
  url: 'https://ondrejnepozitek.github.io/' /* your website url */,
  baseUrl: '/MapGeneration/' /* base url for your project */,
  projectName: 'MapGeneration',
  headerLinks: [
    {doc: 'doc1', label: 'Docs'},
    {doc: 'doc4', label: 'API'},
    {page: 'help', label: 'Help'},
    {blog: true, label: 'Blog'},
  ],
  /* path to images for header/footer */
  /*headerIcon: 'img/docusaurus.svg',*/
  /*footerIcon: 'img/docusaurus.svg',
  favicon: 'img/favicon.png',*/
  /* colors for website */
  colors: {
    primaryColor: '#1a2b34',
    secondaryColor: '#e0e0e0',
  },
  /* custom fonts for website */
  /*fonts: {
    myFont: [
      "Times New Roman",
      "Serif"
    ],
    myOtherFont: [
      "-apple-system",
      "system-ui"
    ]
  },*/
  // This copyright info is used in /core/Footer.js and blog rss/atom feeds.
  copyright:
    'Copyright © ' +
    new Date().getFullYear() +
    ' Your Name or Your Company Name',
  organizationName: 'OndrejNepozitek', // or set an env variable ORGANIZATION_NAME
  projectName: 'MapGeneration', // or set an env variable PROJECT_NAME
  highlight: {
    // Highlight.js theme to use for syntax highlighting in code blocks
    theme: 'default',
  },
  scripts: ['https://buttons.github.io/buttons.js'],
  // You may provide arbitrary config keys to be used as needed by your template.
  repoUrl: 'https://github.com/facebook/test-site',
};

module.exports = siteConfig;
