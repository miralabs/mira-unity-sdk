// Copyright (c) Mira Labs, Inc., 2017. All rights reserved.
//
// Licensed under the MIT License.
// See LICENSE.md at the root of this project for more details.

using System;

namespace UnityEngine.XR.iOS
{
	/// <summary>
	/// Guids for editor-player connections in the Mira Live Preview App
	/// </summary>
	public static class MiraConnectionMessageIds
	{
		public static Guid fromEditorMiraSessionMsgId { get { return new Guid("b4c939fd-6ece-49c8-bd2b-83cfb85b66e6"); } }
		public static Guid gyroMsgId { get { return new Guid("6651e9c9-6d8e-4db4-8b7e-ca36678726c7"); } }
		public static Guid wikiCamMsgId { get { return new Guid("11643971-76fb-445e-91ab-427bffb8eb05"); } }
		public static Guid trackingFoundMsgId { get { return new Guid("44b23c3d-109d-414a-a0ca-c4ce68320827"); } }
		public static Guid trackingLostMsgId { get { return new Guid("4ef85dca-b6d8-45f3-840b-a1fdd4fe1735"); } }
		public static Guid BTRemoteMsgId { get { return new Guid("9c40b9e8-be0c-49ad-aee2-d525622e76d9"); } }
		public static Guid BTRemoteButtonsMsgId { get { return new Guid("d1fc2dd8-502e-4088-b310-51eff60ebe0a"); } }
		public static Guid BTRemoteTouchPadMsgId { get { return new Guid("ed3f5dc0-82dc-4eb6-ae8d-337b4b4c5fdc"); } }
	};

	// This is only necessary because of a bug in Unity 2017.2 and earlier. Kill this when 2017.3 is released.
	public static class MiraSubMessageIds
	{
		public static Guid editorInitMiraRemote { get { return new Guid("2e5d7c45-daef-474d-bf55-1f02f0a10b69"); } }
		public static Guid editorDisconnect { get { return new Guid("45138e0e-151f-4b89-a18c-c828d3439eb6"); } }
		public static Guid screenCaptureJPEGMsgID { get { return new Guid("3f8ae47c-3949-4949-b74f-4deeba378a95"); } }
	};
}